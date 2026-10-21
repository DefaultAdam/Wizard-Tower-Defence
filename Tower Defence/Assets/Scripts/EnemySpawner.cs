using UnityEngine;

// spawner for enemy waves along grid paths
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // enemy prefab to spawn
    public GameObject sprintPrefab;
    public GameObject juggernautPrefab;
    public int sprintStartWave = 3;
    public int juggernautStartWave = 5;
    public float sprintChance = 0.25f;
    public float juggernautChance = 0.15f;
    public Grid grid; // reference to grid
    public float timeBetweenWaves = 3f; // countdown between waves in seconds
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

    private bool isWaveInProgress = false; // check if wave is actively running
    private float currentEnemyHealth = 100f; // total health assigned to spawned enemies
    private int highestMilestone = 0; // highest reached milestone level

    // initialise references on start
    void Start()
    {
        currentEnemyHealth = enemyBaseHealth;

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
        if (isWaveInProgress)
        {
            if (enemiesLeftToSpawn > 0)
            {
                spawnTimer -= Time.deltaTime;

                if (spawnTimer <= 0f)
                {
                    SpawnGroup();
                    spawnTimer = spawnInterval;
                }
            }

            if (HUD.instance != null)
            {
                HUD.instance.SetWaveInProgress();
            }

            if (enemiesLeftToSpawn <= 0 && GameObject.FindWithTag("Enemy") == null)
            {
                isWaveInProgress = false;
                waveTimer = timeBetweenWaves;
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
        isWaveInProgress = true;
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

        if (currentWave == 1)
        {
            currentEnemyHealth = enemyBaseHealth;
        }
        else
        {
            if (HUD.instance != null)
            {
                int currentMilestone = 0;

                if (HUD.instance.currentSP >= 400)
                {
                    currentMilestone = 3;
                }
                else if (HUD.instance.currentSP >= 250)
                {
                    currentMilestone = 2;
                }
                else if (HUD.instance.currentSP >= 150)
                {
                    currentMilestone = 1;
                }

                if (currentMilestone > highestMilestone)
                {
                    highestMilestone = currentMilestone;
                }
            }

            float healthIncrease = levelZeroHealthBonus;

            if (highestMilestone >= 3)
            {
                healthIncrease = levelThreeHealthBonus;
            }
            else if (highestMilestone == 2)
            {
                healthIncrease = levelTwoHealthBonus;
            }
            else if (highestMilestone == 1)
            {
                healthIncrease = levelOneHealthBonus;
            }

            currentEnemyHealth += healthIncrease;
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

    GameObject ChooseEnemyPrefab()
    {
        float roll = Random.value;

        if (juggernautPrefab != null && currentWave >= juggernautStartWave && roll < juggernautChance)
        {
            return juggernautPrefab;
        }

        if (sprintPrefab != null && currentWave >= sprintStartWave && roll >= 1f - sprintChance)
        {
            return sprintPrefab;
        }

        return enemyPrefab;
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
        GameObject enemyObject = Instantiate(ChooseEnemyPrefab(), spawnPosition, Quaternion.identity);

        Health enemyHealth = enemyObject.GetComponent<Health>();
        Enemy enemyScript = enemyObject.GetComponent<Enemy>();

        if (enemyHealth != null)
        {
            float multiplier = 1f;

            if (enemyScript != null)
            {
                multiplier = enemyScript.healthMultiplier;
            }

            enemyHealth.maxHealth = currentEnemyHealth * multiplier;
            enemyHealth.currentHealth = currentEnemyHealth * multiplier;
        }

        EnemyMovement movement = enemyObject.GetComponent<EnemyMovement>();

        if (movement != null)
        {
            movement.grid = grid;
            movement.pathNumber = currentPathIndex;
        }

        currentPathIndex = (currentPathIndex + 1) % 3;
    }
}
