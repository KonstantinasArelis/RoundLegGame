using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    [SerializeField] private float health = 3f;
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private GameObject healthbar;
    [SerializeField] private float knockbackForce = 5f; // Add a knockback force variable

    private LayerMask targetLayerMask;
    private readonly float chaseTargetCooldownSeconds = 0.5f;
    private readonly float speedDeltaToPlayer = 1f;
    private readonly float OUT_OF_BOUNDS_CHECK_SECONDS = 0.5f;
    private readonly float SUICIDE_Y = -10f;

    private GameObject nearestPlayer;
    private Rigidbody rb; // Add a Rigidbody component
	
    private bool isDying = false;
    
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthbar = Instantiate(healthbar, transform.position, healthbar.transform.rotation, transform);
        healthbar.GetComponent<HealthbarController>().SetupHealthbar(health, maxHealth);
        StartCoroutine(SuicideOnOutOfBounds());
        StartCoroutine(ChaseNearestTargetCoroutine());
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (nearestPlayer != null && isDying != true) 
        {
            transform.LookAt(new Vector3(nearestPlayer.transform.position.x, transform.position.y, nearestPlayer.transform.position.z));   
            // move towards player
            transform.position = Vector3.MoveTowards(transform.position, nearestPlayer.transform.position, speedDeltaToPlayer * Time.deltaTime);
        }

        healthbar.transform.position = transform.position + new Vector3(0, -4f, -7);
    }

    void ChaseNearestTarget()
    {
        nearestPlayer = GameObject.FindGameObjectsWithTag("Player")
            .OrderBy(player => (player.transform.position - transform.position).sqrMagnitude)
            .FirstOrDefault();
    }

    private IEnumerator ChaseNearestTargetCoroutine()
    {
        while (true)
        {
            ChaseNearestTarget();
            yield return new WaitForSeconds(chaseTargetCooldownSeconds);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Damager"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("Hitting");
        }
    }

    bool IsDead()
    {
        return health <= 0;
    }

    public void TakeDamage(float damage)
    {
        animator.SetTrigger("Shot");
        health -= damage;
        healthbar.GetComponent<HealthbarController>().OnDamage(damage);

        // Apply knockback
        Vector3 knockbackDirection = (transform.position - nearestPlayer.transform.position).normalized; 
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
	//animator.ResetTrigger("Shot");
        if (IsDead())
        {
        	//animator.SetTrigger("Shot");
        	animator.SetTrigger("Died");
        	StartCoroutine(DelayedSuicide()); // Start the coroutine
        }
    }

    private IEnumerator SuicideOnOutOfBounds()
    {
        for (; ; )
        {
            if (transform.position.y < SUICIDE_Y)
            {
                StartCoroutine(DelayedSuicide()); // Start the coroutine
            }
            yield return new WaitForSeconds(OUT_OF_BOUNDS_CHECK_SECONDS);
        }
    }

    private IEnumerator DelayedSuicide() 
    {
    	
    isDying = true;
    //GetComponent<Collider>().enabled = false; 
    
        yield return new WaitForSeconds(5f);
        Destroy(healthbar);
        Destroy(gameObject);
    }

}
