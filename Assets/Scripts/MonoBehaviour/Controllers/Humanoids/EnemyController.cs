using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

// A lot of logic will be shared among enemies, so override this class if needed
public class EnemyController : MonoBehaviour, IDamagable, IKnockable
{
    private HealthProvider healthProvider;

    [SerializeField] private float maxHealth = 3;
    [SerializeField] private float knockbackForce = 5f; // Add a knockback force variable

    [SerializeField] private int scoreGivenOnDeath = 10;
    [SerializeField] private int xpGivenOnDeath = 10;

    [SerializeField] private float damageGivenOnHit = 1f;
    private readonly float chaseTargetCooldownSeconds = 0.5f;
    private readonly float speedDeltaToPlayer = 1f;
    private readonly float OUT_OF_BOUNDS_CHECK_SECONDS = 0.5f;
    private readonly float SUICIDE_Y = -10f;

    private GameObject nearestPlayer;
    private Rigidbody rb;
    private bool isDying = false;
    private MainHudController mainHudController;

    private Collider myCollider;

    private GameObject bloodSplatterPrefab;

    private float colliderExtentY;
    
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        mainHudController = GameObject.Find("MainHud").GetComponent<MainHudController>();
        healthProvider = new (maxHealth);
        StartCoroutine(SuicideOnOutOfBounds());
        StartCoroutine(ChaseNearestTargetCoroutine());
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();
        bloodSplatterPrefab = (GameObject) Resources.Load("Prefabs/FX/BloodSplatter");
        colliderExtentY = myCollider.bounds.extents.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // if (nearestPlayer != null && isDying != true) 
        // {
        //     transform.LookAt(new Vector3(nearestPlayer.transform.position.x, transform.position.y, nearestPlayer.transform.position.z));
        //     // move towards player
        //     transform.position = Vector3.MoveTowards(transform.position, nearestPlayer.transform.position, speedDeltaToPlayer * Time.deltaTime);
        // }
    }

    void ChaseNearestTarget()
    {
        nearestPlayer = GameObject.FindGameObjectsWithTag("Player")
            .OrderBy(player => (player.transform.position - transform.position).sqrMagnitude)
            .FirstOrDefault();
    }

    private IEnumerator ChaseNearestTargetCoroutine()
    {
        var agent = GetComponent<NavMeshAgent>();

        while (agent.enabled)
        {
            ChaseNearestTarget();
            agent.destination = nearestPlayer.transform.position;
            yield return new WaitForSeconds(chaseTargetCooldownSeconds);
        }
    }

    // WARN: Stay preserves the contact but it could be heavy on the system
    void OnCollisionStay(Collision collision)
    {
        Attack(collision);
    }

    private void Attack(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("Hitting");
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(damageGivenOnHit);
        }

        if (collision.gameObject.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            animator.SetTrigger("Hitting");
            damagable.TakeDamage(damageGivenOnHit);
        }
    }


    public void TakeDamage(float damage)
    {
        animator.SetTrigger("Shot");
        healthProvider.TakeDamage(damage);

        if (healthProvider.IsDead())
        {
            OnDeath();
            return;
        }

        StartCoroutine(TemporaryInvulnerabilityCoroutine());
    }

    public void TakeKnockback(float knockbackForce, Vector3 position)
    {
        // scale it by some factor otherwise you can't really notice it
        knockbackForce *= 50f;
        // don't include y position
        position = new Vector3(position.x, transform.position.y, position.z);
        // make it knockback to back of where the enemy is facing
        Vector3 knockbackDirection = (transform.position - position - transform.forward).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
    }

    public IEnumerator TemporaryInvulnerabilityCoroutine()
    {
        myCollider.enabled = false;
        yield return new WaitForSeconds(0.01f);
        myCollider.enabled = true;
    }

    private void OnDeath()
    {
        GetComponent<NavMeshAgent>().enabled = false;
        animator.SetTrigger("Died");
        mainHudController.AddScore(scoreGivenOnDeath);
        GameObject.FindWithTag("Player").GetComponent<PlayerController>().GainXp(xpGivenOnDeath);
        DelayedSuicide();
    }

    private IEnumerator SuicideOnOutOfBounds()
    {
        for ( ; ; )
        {
            if (transform.position.y < SUICIDE_Y)
            {
                DelayedSuicide(); // Start the coroutine
            }
            yield return new WaitForSeconds(OUT_OF_BOUNDS_CHECK_SECONDS);
        }
    }

    public void OnDiedAnimationFinished()
    {
        var deathMiddlePosition = transform.Find("DeathMiddlePosition").position;
        var splatter = Instantiate(bloodSplatterPrefab, deathMiddlePosition, bloodSplatterPrefab.transform.rotation);
        splatter.GetComponent<GroundSpill>().spillScale = colliderExtentY;
    }

    private void DelayedSuicide() 
    {
        isDying = true;
        Destroy(GetComponent<Collider>());
        GetComponent<Rigidbody>().useGravity = false;
        gameObject.AddComponent<Dissapearer>().Dissapear();
    }

}
