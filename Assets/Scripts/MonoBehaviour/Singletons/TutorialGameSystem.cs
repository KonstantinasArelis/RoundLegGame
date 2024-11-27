using System.Collections;
using UnityEngine;

public class TutorialGameSystem : MonoBehaviour
{
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private GameObject monster1Prefab;
    
    [SerializeField] private float spawnTimeSeconds = 1f;
    private int maxZombiesAtOneTime = 100;

    private SceneFadeController sceneFadeController;

    private MainHudController mainHudController;

    private GameObject player;
    private float spawnFromPlayerRadius = 5f;
    private GameObject ground;
    private Directions groundBorderClip = new Directions{ front = -5f, right = -5f, back = 5f, left = 5f };
    private Directions groundBoundary;

    private int waveTime = 0;
    private readonly int waveEndTime = 60 * 10;

    [SerializeField] public int currentWave = 0;

    float monsterSpawnInterval;
    float monster1SpawnInterval;

    // private int[] zombieCountInWaves = {15,30,50,40,30,10,20,80,100,30,100,300,20,150,20,100,100,20,60,100,100,300,200,300,300,100,150,300,300,300};
    // private int[] monster1CountInWaves = {1,3,5,7,3,10,2,8,1,10,10,30,5,15,20,5,10,20,30,10,10,3,2,30,3,10,15,30,30,30};
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneFadeController.FadeIn(() =>
        {
            groundBoundary = Utility.GetCollidableObjectBoundaries(ground) + groundBorderClip;
            // StartCoroutine(SpawnMonsterCoroutine(zombieCountInWaves, maxZombiesAtOneTime, zombiePrefab));
            // StartCoroutine(SpawnMonsterCoroutine(monster1CountInWaves, maxZombiesAtOneTime, monster1Prefab));
            StartCoroutine(CountdownTimeCoroutine());
        });
    }

    void Awake()
    {
        mainHudController = GameObject.Find("MainHud").GetComponent<MainHudController>();
        sceneFadeController = GameObject.Find("SceneFade").GetComponent<SceneFadeController>();
        player = GameObject.FindWithTag("Player");
        ground = GameObject.FindWithTag("Ground");
    }

    private IEnumerator SpawnMonsterCoroutine(int[] monsterCountInWaves, int maxAtOneTime, GameObject monsterPrefab)
    {
        for ( ; ; )
        {   
            monsterSpawnInterval = 60f/monsterCountInWaves[currentWave];
            yield return new WaitForSeconds(monsterSpawnInterval);
            if (player == null) break;
            if (transform.childCount >= maxAtOneTime) continue;
            Vector3 randomPosition = GetSpawnLocation(monsterPrefab);
            // attach to GameSystem so it's organised
            Instantiate(monsterPrefab, randomPosition, Quaternion.identity, transform);
        }
    }
    
    private Vector3 GetSpawnLocation(GameObject monsterPrefab)
    {
        SkinnedMeshRenderer monsterMesh = monsterPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
        // make sure that it's zero to not mess up center
        monsterPrefab.transform.position = Vector3.zero;

        for ( ; ; )
        {
            // approximating through mesh bounds (their extents should match collider) because prefab extents are 0 for collider
            Vector3 randomPosition = GetRandomSpawnPositionOnGround(monsterMesh);
            Bounds worldBounds = new(
            randomPosition + monsterMesh.bounds.center, 
            Vector3.Scale(monsterMesh.bounds.size, monsterPrefab.transform.lossyScale)
            );
            bool isSpotClearOfObstacles = !Physics.CheckBox(
                worldBounds.center,
                worldBounds.extents,
                monsterPrefab.transform.rotation,
                LayerMask.GetMask("Ground")
            );
            bool isSpotClearOfPlayerRadius =
                Vector2.Distance(
                    new Vector2(randomPosition.x, randomPosition.z),
                    new Vector2(player.transform.position.x, player.transform.position.z))
                > spawnFromPlayerRadius;
            bool isSpotClear = isSpotClearOfObstacles && isSpotClearOfPlayerRadius;
            if (isSpotClear) return randomPosition;
        }
    }

    private Vector3 GetRandomSpawnPositionOnGround(SkinnedMeshRenderer monsterMesh)
    {
        float randomX = Random.Range(groundBoundary.left, groundBoundary.right);
        float randomZ = Random.Range(groundBoundary.back, groundBoundary.front);
        float aboveGroundY = 
            ground.transform.position.y
            + ground.GetComponent<Collider>().bounds.extents.y
            + monsterMesh.bounds.extents.y;
        Vector3 randomPosition = new (
            randomX,
            aboveGroundY,
            randomZ
        );
        return randomPosition;
    }

    private IEnumerator CountdownTimeCoroutine()
    {
        EndMenuManager endMenuManager = GameObject.Find("/EndMenuManager")?.GetComponent<EndMenuManager>();

        while (waveTime < waveEndTime)
        {
            yield return new WaitForSeconds(1f);
            waveTime += 1;
            if(waveTime % 60 == 0 || waveTime==1){
                currentWave++;
                Debug.Log("Current wave: " + currentWave);
                Debug.Log("Zombie spawn interval: " + monsterSpawnInterval);
                Debug.Log("Monster1 spawn interval: " + monster1SpawnInterval);
                // Debug.Log("Zombie count this wave: " + zombieCountInWaves[currentWave]);
                Debug.Log("Current transform amount: " + transform.childCount);
            }
            mainHudController.SetWaveTime(waveEndTime - waveTime);

            if (endMenuManager & transform.childCount == 0)
                endMenuManager.ShowEndMenu(mainHudController.GetScore());
        }

        if (endMenuManager)
            endMenuManager.ShowEndMenu(mainHudController.GetScore());
    }
}
