using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject zombie;
    [SerializeField] private float spawnTimeSeconds = 3f;
    [SerializeField] private Vector3 randomSpawnPositionBounds = new (10f, 0f, 10f);
    private readonly int maxZombiesAtOneTime = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // randomly spawn zombies
        StartCoroutine(SpawnZombiesCoroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator SpawnZombiesCoroutine()
    {
        while (transform.childCount < maxZombiesAtOneTime)
        {
            yield return new WaitForSeconds(spawnTimeSeconds);
            Vector3 randomPosition = new (
                Random.Range(-randomSpawnPositionBounds.x, randomSpawnPositionBounds.x),
                Random.Range(-randomSpawnPositionBounds.y, randomSpawnPositionBounds.y),
                Random.Range(-randomSpawnPositionBounds.z, randomSpawnPositionBounds.z)
            );
            // TODO: exclude player position for zombie spawn cause it messes up camera
            // attach to GameSystem so it's organised
            Instantiate(zombie, randomPosition, Quaternion.identity, transform);
        }
    }
}
