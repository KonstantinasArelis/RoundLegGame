using System.Collections;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private GameObject monster1Prefab;
    [SerializeField] private GameObject player;
    [SerializeField] private float spawnTimeSeconds = 3f;
    private Vector3 worldViewFromPlayerBounds = new (20f, 0, 20f);
    private Transform floorTransform;
    private Vector3 randomSpawnPositionBounds = new (10f, 2f, 10f);
    private int maxZombiesAtOneTime = 100;

    private MainHudController mainHudController;

    private int waveTime = 0;
    private readonly int waveEndTime = 60 * 10;
    [SerializeField] public int currentWave = 0;

    float zombieSpawnInterval;
    float monster1SpawnInterval;

    private int[] zombieCountInWaves = {15,30,50,40,30,10,20,80,100,30,100,300,20,150,20,100,100,20,60,100,100,300,200,300,300,100,150,300,300,300};
    private int[] monster1CountInWaves = {1,3,5,7,3,10,2,8,1,10,10,30,5,15,20,5,10,20,30,10,10,3,2,30,3,10,15,30,30,30};
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // randomly spawn zombies
        StartCoroutine(SpawnZombiesCoroutine());
        StartCoroutine(SpawnMonster1Coroutine());
    }

    void Awake()
    {
        mainHudController = GameObject.Find("MainHud").GetComponent<MainHudController>();
        StartCoroutine(CountdownTimeCoroutine());
    }

    

    private IEnumerator SpawnZombiesCoroutine()
    {
        for ( ; ; )
        {   
            zombieSpawnInterval = 60f/zombieCountInWaves[currentWave];
            yield return new WaitForSeconds(zombieSpawnInterval);
            if (player == null) break;
            if (transform.childCount >= maxZombiesAtOneTime) continue;
            Vector3 randomPosition = GetSpawnLocation();
            // attach to GameSystem so it's organised
            Instantiate(zombiePrefab, randomPosition, Quaternion.identity, transform);
        }
    }

    private IEnumerator SpawnMonster1Coroutine()
    {
        for ( ; ; )
        {   
            monster1SpawnInterval = 60f/monster1CountInWaves[currentWave];
            yield return new WaitForSeconds(monster1SpawnInterval);
            if (player == null) break;
            if (transform.childCount >= maxZombiesAtOneTime) continue;
            Vector3 randomPosition = GetSpawnLocation();
            // attach to GameSystem so it's organised
            Instantiate(monster1Prefab, randomPosition, Quaternion.identity, transform);
        }
    }
    
    private Vector3 GetSpawnLocation()
    {
        float randomX = Random.Range(-randomSpawnPositionBounds.x, randomSpawnPositionBounds.x);
        float randomZ = Random.Range(-randomSpawnPositionBounds.z, randomSpawnPositionBounds.z);
        Vector3 randomPosition = new (
        randomX < 0
            ? player.transform.position.x + randomX - worldViewFromPlayerBounds.x
            : player.transform.position.x + randomX + worldViewFromPlayerBounds.x,
        randomSpawnPositionBounds.y,
        randomZ < 0
            ? player.transform.position.z + randomZ - worldViewFromPlayerBounds.z
            : player.transform.position.z + randomZ + worldViewFromPlayerBounds.z
        );
        return randomPosition;
    }

    private IEnumerator CountdownTimeCoroutine()
    {
        while (waveTime < waveEndTime)
        {
            yield return new WaitForSeconds(1f);
            waveTime += 1;
            if(waveTime % 60 == 0 || waveTime==1){
                currentWave++;
                Debug.Log("Current wave: " + currentWave);
                Debug.Log("Zombie spawn interval: " + zombieSpawnInterval);
                Debug.Log("Monster1 spawn interval: " + monster1SpawnInterval);
                Debug.Log("Zombie count this wave: " + zombieCountInWaves[currentWave]);
                Debug.Log("Current transform amount: " + transform.childCount);
            }
            mainHudController.SetWaveTime(waveTime);
        }
    }
}
