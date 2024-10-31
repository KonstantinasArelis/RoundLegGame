using System.Collections;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private GameObject player;
    [SerializeField] private float spawnTimeSeconds = 3f;
    private Vector3 worldViewFromPlayerBounds = new (20f, 0, 20f);
    private Vector3 randomSpawnPositionBounds = new (10f, 2f, 10f);
    private int maxZombiesAtOneTime = 10;

    private MainHudController mainHudController;

    private int waveTime = 0;
    private readonly int waveEndTime = 60 * 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // randomly spawn zombies
        StartCoroutine(SpawnZombiesCoroutine());
    }

    void Awake()
    {
        mainHudController = GameObject.Find("MainHud").GetComponent<MainHudController>();
        StartCoroutine(CountdownTimeCoroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator SpawnZombiesCoroutine()
    {
        for ( ; ; )
        {
            yield return new WaitForSeconds(spawnTimeSeconds);
            if (player == null) break;
            if (transform.childCount >= maxZombiesAtOneTime) continue;
            float randomX = Random.Range(-randomSpawnPositionBounds.x, randomSpawnPositionBounds.x);
            float randomZ = Random.Range(-randomSpawnPositionBounds.z, randomSpawnPositionBounds.z);
            Vector3 randomPosition = new (
                randomX < 0
                    ? player.transform.position.x + randomX - worldViewFromPlayerBounds.x
                    : player.transform.position.x + randomX + worldViewFromPlayerBounds.x,
                randomSpawnPositionBounds.y,
                randomZ < 0
                    ? player.transform.position.z + randomX - worldViewFromPlayerBounds.z
                    : player.transform.position.z + randomX + worldViewFromPlayerBounds.z
            );
            // attach to GameSystem so it's organised
            Instantiate(zombiePrefab, randomPosition, Quaternion.identity, transform);
        }
    }

    private IEnumerator CountdownTimeCoroutine()
    {
        while (waveTime < waveEndTime)
        {
            yield return new WaitForSeconds(1f);
            waveTime += 1;
            mainHudController.SetWaveTime(waveTime);
        }
    }
}
