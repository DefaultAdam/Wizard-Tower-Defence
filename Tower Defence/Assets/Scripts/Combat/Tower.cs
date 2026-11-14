using UnityEngine;
using System.Collections.Generic;

// targeting priority options
public enum TargetPriority
{
    First,
    Closest,
    Strongest,
    Weakest
}

// specialisation branch choices
public enum Branch
{
    None,
    Physical,
    Mechanic,
    Magic,
    Imaginary
}

// tower combat and attack script
public class Tower : MonoBehaviour
{
    public float attackRange = 6f; // attack range distance
    public float attackCooldown = 1f; // seconds between attacks
    public float damage = 25f; // damage dealt per attack

    public float damagePerWave = 5f; // extra damage gained each round
    public float aoeRadius = 0f; // splash attack radius
    public float aoeDamagePercent = 0.5f; // splash damage percentage
    public TargetPriority targetPriority = TargetPriority.Closest; // targeting priority criteria

    public GameObject projectilePrefab; // projectile prefab to shoot
    public Transform firePoint; // spawn point for projectiles
    public Transform towerHead; // rotating part of the tower
    public float rotationSpeed = 10f; // rotation speed towards target

    public Branch branch = Branch.None; // chosen specialisation branch
    public int level = 0; // troop level tier
    public bool isSuper = false; // true when this unit has been created by fusion

    public float boltCooldown = 0.8f; // physical apprentice bolts fire this often, seperate to the normal attack
    public float veteranBoltCooldown = 0.5f; // veterans fire the same bolts but more often
    public float boltDamagePercent = 0.5f; // each bolt does half the troops damage since theres loads of them
    public float boltSpeedMultiplier = 1.75f; // bolts fly 75% faster than normal stones
    public float boltSizeMultiplier = 1.8f; // and theyre bigger so you can tell them apart
    private float boltTimer = 0f; // countdown till the next physical bolt

    public float wallCooldown = 6f; // imaginary apprentices build a wall this often
    public float veteranWallCooldown = 4f; // veterans rebuild quicker
    public float wallHealth = 100f; // how tough an apprentice wall is
    public float veteranWallHealth = 200f; // veteran walls take twice as much to break
    public int wallCellsAhead = 2; // how many cells in front of the target the wall goes
    private float wallTimer = 0f; // countdown till the next wall can go down
    private ImaginaryWall currentWall; // the wall this troop has out right now, only one at a time
    public float wallLifetime = 8f; // walls disappear on their own after this if nobody breaks them
    public float veteranWallLifetime = 10f; // veteran walls stick around longer
    private float wallLifeTimer = 0f; // time left before the current wall vanishes
    private float currentWallHeight = 1f; // preserve the height chosen when this wall was built

    private float cooldownTimer = 0f; // timer to track attack cooldown
    private GameObject currentTarget; // current target enemy

    // apply troop colour on start
    void Start()
    {
        ApplyColour();
    }

    // get colour for a specialisation branch
    public Color GetBranchColour()
    {
        if (branch == Branch.Physical)
        {
            return new Color(0.78f, 0.6f, 0.42f);
        }

        if (branch == Branch.Mechanic)
        {
            return new Color(0.45f, 0.56f, 0.68f);
        }

        if (branch == Branch.Magic)
        {
            return new Color(0.6f, 0.3f, 0.85f);
        }

        if (branch == Branch.Imaginary)
        {
            return new Color(1f, 0.85f, 0.2f);
        }

        return Color.white;
    }

    // apply branch colour to all troop renderers
    public void ApplyColour(bool fusionSelected = false)
    {
        Color colour = fusionSelected ? Color.green : GetBranchColour();

        foreach (Renderer troopRenderer in GetComponentsInChildren<Renderer>())
        {
            troopRenderer.material.color = colour;
        }
    }

    // upgrade recruit to apprentice branch with upgraded combat stats
    public void UpgradeToApprentice(Branch newBranch, Tower apprenticeTemplate)
    {
        branch = newBranch;
        level = 1;

        if (apprenticeTemplate != null)
        {
            attackRange = apprenticeTemplate.attackRange;
            attackCooldown = apprenticeTemplate.attackCooldown;
            damage = apprenticeTemplate.damage;
            damagePerWave = apprenticeTemplate.damagePerWave;
            aoeRadius = apprenticeTemplate.aoeRadius;
            aoeDamagePercent = apprenticeTemplate.aoeDamagePercent;
        }
        else
        {
            attackRange = 7f;
            attackCooldown = 2f;
            damage = 35f;
            damagePerWave = 5f;
            aoeRadius = 2f;
            aoeDamagePercent = 0.5f;
        }

        ApplyColour();
    }

    // upgrade recruit to apprentice branch
    public void UpgradeToApprentice(Branch newBranch)
    {
        UpgradeToApprentice(newBranch, null);
    }

    // upgrade apprentice to veteran with upgraded stats
    public void UpgradeToVeteran(Tower veteranTemplate)
    {
        level = 2;

        if (veteranTemplate != null)
        {
            attackRange = veteranTemplate.attackRange * 1.25f;
            attackCooldown = veteranTemplate.attackCooldown * 0.75f;
            damage = veteranTemplate.damage * 1.5f;
            damagePerWave = veteranTemplate.damagePerWave * 1.5f;
            aoeRadius = veteranTemplate.aoeRadius * 1.25f;
            aoeDamagePercent = veteranTemplate.aoeDamagePercent;
        }
        else
        {
            attackRange = 8.5f;
            attackCooldown = 1.5f;
            damage = 50f;
            damagePerWave = 7.5f;
            aoeRadius = 2.5f;
            aoeDamagePercent = 0.5f;
        }

        ApplyColour();
    }

    // upgrade veteran to master with upgraded stats
    public void UpgradeToMaster(Tower masterTemplate)
    {
        if (masterTemplate != null)
        {
            attackRange = masterTemplate.attackRange;
            attackCooldown = masterTemplate.attackCooldown;
            damage = masterTemplate.damage;
            damagePerWave = masterTemplate.damagePerWave;
        }

        level = 3;
        ApplyColour();
    }

    // check three units without changing or consuming them
    public static bool CanFuse(Tower first, Tower second, Tower third, out string reason)
    {
        reason = "";

        if (first == null || second == null || third == null)
        {
            reason = "Select three units";
            return false;
        }

        if (first == second || first == third || second == third)
        {
            reason = "Select three different units";
            return false;
        }

        if (first.level != 3 || second.level != 3 || third.level != 3)
        {
            reason = "All three units must be Level 3";
            return false;
        }

        if (first.isSuper || second.isSuper || third.isSuper)
        {
            reason = "Super Apprentices cannot fuse again";
            return false;
        }

        if (first.branch == Branch.None || first.branch != second.branch || first.branch != third.branch)
        {
            reason = "All three units must have the same specialisation branch";
            return false;
        }

        return true;
    }

    // get display name for troop
    public string GetTroopName()
    {
        if (level <= 0)
        {
            return "Recruit";
        }

        if (branch == Branch.None)
        {
            if (level == 1)
            {
                return "Apprentice";
            }

            if (level == 2)
            {
                return "Veteran";
            }

            if (level == 3)
            {
                return "Master";
            }

            return "Recruit";
        }

        if (level == 1)
        {
            return branch + " Apprentice";
        }

        if (level == 2)
        {
            return branch + " Veteran";
        }

        if (level == 3)
        {
            if (isSuper)
            {
                return "Super " + branch;
            }

            return branch + " Master";
        }

        return branch + " Recruit";
    }

    // get current damage including wave bonus
    public float GetCurrentDamage()
    {
        float totalDamage = damage;

        if (HUD.instance != null && HUD.instance.currentWave > 1)
        {
            totalDamage += (HUD.instance.currentWave - 1) * damagePerWave;
        }

        return totalDamage;
    }

    // get combat stats text for display
    public string GetStatsText()
    {
        if (isSuper && branch == Branch.Physical && level == 3)
        {
            float beamDamage = GetCurrentDamage() * 6f;
            return "Rail-Blast: " + beamDamage + " | Interval: " + attackCooldown + "s"
                + "\nRange: " + attackRange + " | Pierces: 2"
                + "\nBlast: " + (beamDamage * 0.5f) + " | Radius: " + aoeRadius;
        }

        if (isSuper && branch == Branch.Mechanic && level == 3)
        {
            Projectile template = projectilePrefab != null ? projectilePrefab.GetComponent<Projectile>() : null;
            int throwCells = template != null ? template.veteranThrowCells * 6 : 18;
            return "Slam: " + GetCurrentDamage() + " | Interval: " + attackCooldown + "s"
                + "\nRadius: " + attackRange + " | Pushback: " + throwCells + " tiles"
                + "\nThrow Duration: 1.5x";
        }

        if (isSuper && branch == Branch.Magic && level == 3)
        {
            Projectile template = projectilePrefab != null ? projectilePrefab.GetComponent<Projectile>() : null;
            float duration = template != null ? template.veteranFreezeTime * 6f : 15f;
            return "Storm: " + GetCurrentDamage() + " | Interval: " + attackCooldown + "s"
                + "\nRadius: 6 tiles | Freeze: " + duration + "s";
        }

        string text = "Damage: " + GetCurrentDamage() + "\nFire Rate: " + attackCooldown + "s\nRange: " + attackRange;

        if (aoeRadius > 0f)
        {
            text += "\nSplash: " + aoeRadius;
        }

        return text;
    }

    // update target tracking and attack cooldown
    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (!IsTargetValid(currentTarget))
        {
            FindTarget();
        }

        if (currentTarget != null)
        {
            RotateTowardsTarget();

            if (cooldownTimer <= 0f)
            {
                Attack(currentTarget);
            }
        }

        //PHYSICAL BRANCH


        // physical troops also shoot energy bolts on their own timer
        if (branch == Branch.Physical && level >= 1 && !isSuper)
        {
            UpdateBolts();
        }

        // imaginary troops build walls on the path
        if (branch == Branch.Imaginary && level >= 1)
        {
            UpdateWalls();
        }
    }

    // waits for the troops last wall to be gone, then counts down and builds a new one
    void UpdateWalls()
    {
        // still got a wall out, keep it updated and dont make another
        if (currentWall != null)
        {
            UpdateCurrentWall();
            return;
        }

        if (wallTimer > 0f)
        {
            wallTimer -= Time.deltaTime;
        }

        if (currentTarget != null && wallTimer <= 0f)
        {
            BuildWall(currentTarget);
        }
    }

    // puts a wall on the path a couple of cells in front of the target so it walks into it
    void BuildWall(GameObject target)
    {
        EnemyMovement movement = target.GetComponent<EnemyMovement>();

        if (movement == null)
        {
            return;
        }

        Vector2Int wallCell;

        // target's too close to the end of its path, try again next frame
        if (!movement.GetCellAhead(wallCellsAhead, out wallCell))
        {
            return;
        }

        // someone elses wall is already there
        if (ImaginaryWall.GetWallAt(wallCell) != null)
        {
            return;
        }

        // just a yellow block for now, slightly smaller than the cell so you can see the gaps
        GameObject wallObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(wallObject.GetComponent<Collider>());

        float cellSize = movement.grid.cellSize;
        currentWallHeight = isSuper && level == 3 ? 2f : 1f;
        wallObject.transform.position = movement.grid.GetCellCenter(wallCell) + new Vector3(0f, currentWallHeight * 0.5f, 0f);
        wallObject.transform.localScale = new Vector3(cellSize * 0.9f, currentWallHeight, cellSize * 0.9f);
        wallObject.GetComponent<Renderer>().material.color = GetBranchColour();

        float health = wallHealth;
        wallTimer = wallCooldown;

        if (level >= 2)
        {
            health = veteranWallHealth;
            wallTimer = veteranWallCooldown;
        }

        if (isSuper && level == 3)
        {
            health *= 6f;
        }

        currentWall = wallObject.AddComponent<ImaginaryWall>();
        currentWall.Setup(wallCell, health);

        wallLifeTimer = wallLifetime;

        if (level >= 2)
        {
            wallLifeTimer = veteranWallLifetime;
        }
    }

    // shrinks the wall as it gets damaged and gets rid of it when its time is up
    void UpdateCurrentWall()
    {
        wallLifeTimer -= Time.deltaTime;

        if (wallLifeTimer <= 0f)
        {
            currentWall.Break();
            return;
        }

        // squash it down to show how much health is left, never fully flat so you can still see it
        float healthLeft = currentWall.currentHealth / currentWall.maxHealth;
        float height = Mathf.Max(0.2f, healthLeft) * currentWallHeight;

        Vector3 scale = currentWall.transform.localScale;
        Vector3 position = currentWall.transform.position;

        // keep the bottom on the ground while it shrinks, so move it down by half the height it lost
        position.y += (height - scale.y) * 0.5f;
        scale.y = height;

        currentWall.transform.localScale = scale;
        currentWall.transform.position = position;
    }

    // checks bolt timer and fires when its ready, independant from the normal attack cooldown
    void UpdateBolts()
    {
        if (boltTimer > 0f)
        {
            boltTimer -= Time.deltaTime;
        }

        if (currentTarget != null && boltTimer <= 0f)
        {
            FireBolt(currentTarget);
        }
    }

    // shoots one energy bolt at the target
    void FireBolt(GameObject target)
    {
        boltTimer = boltCooldown;

        // veteran and up = faster bolts
        if (level >= 2)
        {
            boltTimer = veteranBoltCooldown;
        }

        if (projectilePrefab == null)
        {
            return;
        }

        Vector3 spawnPosition = transform.position;

        if (firePoint != null)
        {
            spawnPosition = firePoint.position;
        }

        GameObject boltObject = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Projectile bolt = boltObject.GetComponent<Projectile>();

        if (bolt != null)
        {
            bolt.damage = GetCurrentDamage() * boltDamagePercent;

            // bolts are just a straight hit, no splash
            bolt.aoeRadius = 0f;
            bolt.speed *= boltSpeedMultiplier;
            bolt.SetTarget(target.transform);
        }

        // making it big and brown so it looks like a physical energy bolt, different from regular
        boltObject.transform.localScale *= boltSizeMultiplier;

        foreach (Renderer boltRenderer in boltObject.GetComponentsInChildren<Renderer>())
        {
            boltRenderer.material.color = GetBranchColour();
        }













        
    }

    // check if current target is alive and in range
    bool IsTargetValid(GameObject target)
    {
        if (target == null)
        {
            return false;
        }

        Health enemyHealth = target.GetComponent<Health>();

        if (enemyHealth != null && enemyHealth.isDead)
        {
            return false;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance > attackRange)
        {
            return false;
        }

        return true;
    }

    // find best target in range using overlap sphere
    void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);
        currentTarget = null;

        float bestDistance = attackRange;
        float highestHealth = -1f;
        float lowestHealth = float.MaxValue;
        float furthestDistance = -1f;

        foreach (Collider hitCollider in colliders)
        {
            if (hitCollider == null || !hitCollider.CompareTag("Enemy"))
            {
                continue;
            }

            GameObject enemy = hitCollider.gameObject;
            Health enemyHealth = enemy.GetComponent<Health>();

            if (enemyHealth != null && enemyHealth.isDead)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance > attackRange)
            {
                continue;
            }

            if (targetPriority == TargetPriority.Closest)
            {
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    currentTarget = enemy;
                }
            }
            else if (targetPriority == TargetPriority.First)
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                float progress = 0f;

                if (enemyScript != null)
                {
                    progress = enemyScript.distanceTravelled;
                }

                if (progress > furthestDistance)
                {
                    furthestDistance = progress;
                    currentTarget = enemy;
                }
            }
            else if (targetPriority == TargetPriority.Strongest)
            {
                float currentHealth = 0f;

                if (enemyHealth != null)
                {
                    currentHealth = enemyHealth.currentHealth;
                }

                if (currentHealth > highestHealth)
                {
                    highestHealth = currentHealth;
                    currentTarget = enemy;
                }
            }
            else if (targetPriority == TargetPriority.Weakest)
            {
                float currentHealth = float.MaxValue;

                if (enemyHealth != null)
                {
                    currentHealth = enemyHealth.currentHealth;
                }

                if (currentHealth < lowestHealth)
                {
                    lowestHealth = currentHealth;
                    currentTarget = enemy;
                }
            }
        }
    }

    // rotate tower to face current target
    void RotateTowardsTarget()
    {
        Transform targetTransform = transform;

        if (towerHead != null)
        {
            targetTransform = towerHead;
        }

        Vector3 direction = currentTarget.transform.position - targetTransform.position;
        direction.y = 0f;

        if (direction.magnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            targetTransform.rotation = Quaternion.Lerp(targetTransform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void FireRailBlast(GameObject target)
    {
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        spawnPosition.y = target.transform.position.y;
        Vector3 direction = target.transform.position - spawnPosition;
        if (direction.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        float cellSize = 1f;
        if (GridSelector.instance != null && GridSelector.instance.grid != null)
        {
            cellSize = GridSelector.instance.grid.cellSize;
        }

        GameObject blast = GameObject.CreatePrimitive(PrimitiveType.Cube);
        blast.name = "Piercing Rail-Blast";
        Destroy(blast.GetComponent<Collider>());
        blast.transform.position = spawnPosition;
        blast.transform.rotation = Quaternion.LookRotation(direction);
        blast.transform.localScale = new Vector3(cellSize * 0.5f, cellSize * 0.5f, cellSize);
        blast.GetComponent<Renderer>().material.color = GetBranchColour();

        RailBlastProjectile projectile = blast.AddComponent<RailBlastProjectile>();
        projectile.damage = GetCurrentDamage() * 6f;
        projectile.maxDistance = attackRange;
        projectile.explosionRadius = aoeRadius;
        projectile.beamWidth = cellSize * 0.5f;
    }

    private void GravitationalSlam()
    {
        int throwCells = 3;
        if (projectilePrefab != null)
        {
            Projectile template = projectilePrefab.GetComponent<Projectile>();
            if (template != null)
            {
                throwCells = template.veteranThrowCells;
            }
        }

        List<Health> hitEnemies = new List<Health>();
        foreach (Collider hit in Physics.OverlapSphere(transform.position, attackRange))
        {
            Health enemy = hit.GetComponentInParent<Health>();
            if (enemy == null || enemy.isDead || !enemy.CompareTag("Enemy") || hitEnemies.Contains(enemy))
            {
                continue;
            }

            hitEnemies.Add(enemy);
            enemy.TakeDamage(GetCurrentDamage());
            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (!enemy.isDead && movement != null)
            {
                movement.PushBack(throwCells * 6, 1.5f);
            }
        }
    }

    private void StasisStorm()
    {
        Projectile template = projectilePrefab != null ? projectilePrefab.GetComponent<Projectile>() : null;
        float duration = template != null ? template.veteranFreezeTime * 6f : 15f;
        Material bubbleMaterial = template != null ? template.bubbleMaterial : null;
        float radius = 6f; // 6x a one-tile baseline radius
        if (GridSelector.instance != null && GridSelector.instance.grid != null)
        {
            radius *= GridSelector.instance.grid.cellSize;
        }

        List<Health> hitEnemies = new List<Health>();
        foreach (Collider hit in Physics.OverlapSphere(transform.position, radius))
        {
            Health enemy = hit.GetComponentInParent<Health>();
            if (enemy == null || enemy.isDead || !enemy.CompareTag("Enemy") || hitEnemies.Contains(enemy))
            {
                continue;
            }

            hitEnemies.Add(enemy);
            enemy.TakeDamage(GetCurrentDamage());
            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (!enemy.isDead && movement != null)
            {
                movement.Bubble(duration, bubbleMaterial);
            }
        }
    }

    // attack target by spawning a projectile
    void Attack(GameObject target)
    {
        cooldownTimer = attackCooldown;

        if (isSuper && branch == Branch.Imaginary && level == 3)
        {
            return; // walls are built by UpdateWalls on their own timer
        }

        if (isSuper && branch == Branch.Magic && level == 3)
        {
            StasisStorm();
            return;
        }

        if (isSuper && branch == Branch.Mechanic && level == 3)
        {
            GravitationalSlam();
            return;
        }

        if (isSuper && branch == Branch.Physical && level == 3)
        {
            FireRailBlast(target);
            return;
        }

        if (projectilePrefab != null)
        {
            Vector3 spawnPosition = transform.position;
            Quaternion spawnRotation = transform.rotation;

            if (firePoint != null)
            {
                spawnPosition = firePoint.position;
                spawnRotation = firePoint.rotation;
            }

            GameObject projectileObject = Instantiate(projectilePrefab, spawnPosition, spawnRotation);
            Projectile projectile = projectileObject.GetComponent<Projectile>();

            if (projectile != null)
            {
                projectile.damage = GetCurrentDamage();
                projectile.aoeRadius = aoeRadius;
                projectile.aoeDamagePercent = aoeDamagePercent;
                projectile.branch = branch;
                projectile.level = level;
                projectile.SetTarget(target.transform);
            }
        }
    }

    // draw attack range sphere in editor scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
