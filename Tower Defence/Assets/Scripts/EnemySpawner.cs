using UnityEngine;

// spawner for enemy waves along grid paths
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // enemy prefab to spawn
    public Grid grid; // reference to grid
    public float timeBetweenWaves = 15f; // time between waves in seconds
    public float spawnInterval = 0.4f; // seconds between individual enemy spawns

    public float spawnRateIncrease = 0.05f; // reduction in spawn interval each wave

    public float minimumSpawnInterval = 0.15f; // lowest spawn interval limit
    public int enemiesPerWave = 9; // number of enemies in first wave

    public int baseEnemyBonus = 2; // extra enemies added each wave before round five

    public int roundFiveEnemyBonus = 3; // extra enemies added each wave starting round five

    public int roundTenEnemyBonus = 4; // extra enemies added each wave starting round ten

    public int roundFifteenEnemyBonus = 5; // extra enemies added each wave starting round fifteen
    public int currentWave = 0; // current wave counter

    public int groupSize = 1; // number of enemies to spawn in each group
    public float enemyBaseHealth = 100f; // starting enemy health
    public float levelZeroHealthBonus = 10f; // health increase per round at level zero
    public float levelOneHealthBonus = 20f; // health increase per round at level one
    public float levelTwoHealthBonus = 30f; // health increase per round at level two
    public float levelThreeHealthBonus = 50f; // health increase per round at level three

    private float waveTimer = 3f; // countdown timer to next wave
    private float spawnTimer = 0f; // countdown timer to next enemy spawn
    private int enemiesLeftToSpawn = 0; // remaining enemies to spawn in wave
    private int currentPathIndex = 0; // current path index to spawn on

    // initialise references on start
    void Start()
    {
        if (grid == null)
        {
            grid = FindObjectOfType<Grid>();
        }

        if (HUD.instance != null)
        {
            HUD.instance.SetWave(currentWave);
            HUD.instance.SetTimer(waveTimer);
        }
    }

    // handle wave timers and enemy spawning
    void Update()
    {
        if (enemiesLeftToSpawn > 0)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0f)
            {
                SpawnGroup();
                spawnTimer = spawnInterval;
            }

            return;
        }

        waveTimer -= Time.deltaTime;

        if (HUD.instance != null)
        {
            HUD.instance.SetTimer(waveTimer);
        }

        if (waveTimer <= 0f)
        {
            StartNextWave();
        }
    }

    // calculate total enemies to spawn in wave
    int CalculateWaveEnemyCount(int wave)
    {
        int count = enemiesPerWave;

        for (int i = 2; i <= wave; i++)
        {
            if (i >= 15)
            {
                count += roundFifteenEnemyBonus;
            }
            else if (i >= 10)
            {
                count += roundTenEnemyBonus;
            }
            else if (i >= 5)
            {
                count += roundFiveEnemyBonus;
            }
            else
            {
                count += baseEnemyBonus;
            }
        }

        return count;
    }

    // begin next enemy wave
    void StartNextWave()
    {
        currentWave++;
        int totalEnemies = CalculateWaveEnemyCount(currentWave);

        if (totalEnemies < 1)
        {
            totalEnemies = 1;
        }

        enemiesLeftToSpawn = totalEnemies;
        waveTimer = timeBetweenWaves;
        spawnTimer = 0f;

        spawnInterval -= spawnRateIncrease;

        if (spawnInterval < minimumSpawnInterval)
        {
            spawnInterval = minimumSpawnInterval;
        }

        if (HUD.instance != null)
        {
            HUD.instance.SetWave(currentWave);
        }

        if (currentWave > 1 && currentWave % 2 == 0)
        {
            groupSize++;
        }

        if (GridSelector.instance != null)
        {
            GridSelector.instance.UpdateAllowedTowers(currentWave);
        }
    }

    // spawn group of enemies on paths
    void SpawnGroup()
    {
        for (int i = 0; i < groupSize; i++)
        {
            if (enemiesLeftToSpawn > 0)
            {
                SpawnEnemy();
                enemiesLeftToSpawn--;
            }
        }
    }

    // spawn single enemy on path
    void SpawnEnemy()
    {
        if (enemyPrefab == null || grid == null)
        {
            return;
        }

        Vector2Int[] path = grid.GetPath(currentPathIndex);

        if (path == null || path.Length == 0)
        {
            return;
        }

        Vector3 spawnPosition = grid.GetCellCenter(path[0]);
        GameObject enemyObject = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        EnemyMovement movement = enemyObject.GetComponent<EnemyMovement>();

        if (movement != null)
        {
            movement.grid = grid;
            movement.pathNumber = currentPathIndex;
        }

        currentPathIndex = (currentPathIndex + 1) % 3;
    }
}
