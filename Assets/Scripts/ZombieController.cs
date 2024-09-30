using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    [SerializeField] private float health = 3f;
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private GameObject healthbar;
    private LayerMask targetLayerMask;
    private readonly float chaseTargetCooldownSeconds = 0.5f;
    private readonly float speedDeltaToPlayer = 1f;
    private readonly float OUT_OF_BOUNDS_CHECK_SECONDS = 0.5f;
    private readonly float SUICIDE_Y = -10f;

    private GameObject nearestPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // how much the zombie moves towards the player at an instant
        healthbar = Instantiate(healthbar, transform.position, healthbar.transform.rotation, transform);
        healthbar.GetComponent<HealthbarController>().SetupHealthbar(health, maxHealth);
        StartCoroutine(SuicideOnOutOfBounds());
        StartCoroutine(ChaseNearestTargetCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(new Vector3(nearestPlayer.transform.position.x, transform.position.y, nearestPlayer.transform.position.z));
        // move towards player
        transform.position = Vector3.MoveTowards(transform.position, nearestPlayer.transform.position, speedDeltaToPlayer * Time.deltaTime);

        // magic offset because UI positions weirdly as an object
        // TODO: make it look uniform to in respect to camera
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
            // TODO: check what collider it was and how much damage it inflicts (maybe creater a Damager.GetDamage() interface?)
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
    }

    bool IsDead()
    {
        return health <= 0;
    }

    void TakeDamage(float damage)
    {
        health -= damage;
        healthbar.GetComponent<HealthbarController>().OnDamage(damage);
        if (IsDead())
        {
            Suicide();
        }
    }

    private IEnumerator SuicideOnOutOfBounds()
    {
        for ( ; ; ) {
            if (transform.position.y < SUICIDE_Y)
            {
                Suicide();
            }
            yield return new WaitForSeconds(OUT_OF_BOUNDS_CHECK_SECONDS);
        }
    }

    private void Suicide()
    {
        Destroy(healthbar);
        Destroy(gameObject);
    }
}
