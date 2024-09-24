using System;
using System.Collections;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    private GameObject player;
    private readonly float OUT_OF_BOUNDS_CHECK_SECONDS = 0.5f;
    private readonly float DELTA_TO_PLAYER = 1e-3f;
    private readonly float SUICIDE_HEIGHT = -10f;
    private float speedToPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        speedToPlayer = DELTA_TO_PLAYER * speed;
    }

    // Update is called once per frame
    void Update()
    {
        // move towards player
        transform.position = Vector3.MoveTowards(
            transform.position, player.transform.position, speedToPlayer);
        StartCoroutine(SuicideOnOutOfBounds());
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Damager"))
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }

    private IEnumerator SuicideOnOutOfBounds()
    {
        for ( ; ; ) {
            if (transform.position.y < SUICIDE_HEIGHT)
            {
                Destroy(gameObject);
            }
            yield return new WaitForSeconds(OUT_OF_BOUNDS_CHECK_SECONDS);
        }
    }
}
