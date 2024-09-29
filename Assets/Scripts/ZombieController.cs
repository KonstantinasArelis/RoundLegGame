using System;
using System.Collections;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    [SerializeField] private float health = 3f;
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private GameObject healthbar;
    private GameObject player;
    private readonly float speedDeltaToPlayer = 1f;
    private readonly float OUT_OF_BOUNDS_CHECK_SECONDS = 0.5f;
    private readonly float SUICIDE_Y = -10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        // how much the zombie moves towards the player at an instant
        healthbar = Instantiate(healthbar, transform.position, healthbar.transform.rotation, transform);
        healthbar.GetComponent<HealthbarController>().SetupHealthbar(health, maxHealth);
        StartCoroutine(SuicideOnOutOfBounds());
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        // move towards player
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speedDeltaToPlayer * Time.deltaTime);

        // magic offset because UI positions weirdly as an object
        // TODO: make it look uniform to in respect to camera
        healthbar.transform.position = transform.position + new Vector3(0, -4f, -7);
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
