using UnityEngine;

// spawner for enemy waves along grid paths
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // enemy prefab to spawn
    public Grid grid; // reference to grid
    public float timeBetweenWaves = 15f; // time between waves in seconds
    public float spawnInterval = 1f; // seconds between individual enemy spawns
    public int enemiesPerWave = 5; // number of enemies per wave
    public int currentWave = 0; // current wave counter

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
                SpawnEnemy();
                spawnTimer = spawnInterval;
                enemiesLeftToSpawn--;
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

    // begin next enemy wave
    void StartNextWave()
    {
        currentWave++;
        enemiesLeftToSpawn = enemiesPerWave + (currentWave * 2);
        waveTimer = timeBetweenWaves;
        spawnTimer = 0f;

        if (HUD.instance != null)
        {
            HUD.instance.SetWave(currentWave);
        }

        if (GridSelector.instance != null)
        {
            GridSelector.instance.UpdateAllowedTowers(currentWave);
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
