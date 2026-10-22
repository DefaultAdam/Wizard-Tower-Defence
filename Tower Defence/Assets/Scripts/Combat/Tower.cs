using UnityEngine;

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
    public TargetPriority targetPriority = TargetPriority.Closest; // targeting priority criteria

    public GameObject projectilePrefab; // projectile prefab to shoot
    public Transform firePoint; // spawn point for projectiles
    public Transform towerHead; // rotating part of the tower
    public float rotationSpeed = 10f; // rotation speed towards target

    public Branch branch = Branch.None; // chosen specialisation branch
    public int level = 0; // troop level tier

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
    public void ApplyColour()
    {
        Color colour = GetBranchColour();

        foreach (Renderer troopRenderer in GetComponentsInChildren<Renderer>())
        {
            troopRenderer.material.color = colour;
        }
    }

    // upgrade recruit to apprentice branch
    public void UpgradeToApprentice(Branch newBranch)
    {
        branch = newBranch;
        level = 1;
        ApplyColour();
    }

    // upgrade apprentice to veteran with upgraded stats
    public void UpgradeToVeteran(Tower veteranTemplate)
    {
        if (veteranTemplate != null)
        {
            attackRange = veteranTemplate.attackRange;
            attackCooldown = veteranTemplate.attackCooldown;
            damage = veteranTemplate.damage;
            damagePerWave = veteranTemplate.damagePerWave;
        }

        level = 2;
        ApplyColour();
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

            return "Veteran";
        }

        if (level == 1)
        {
            return branch + " Apprentice";
        }

        return branch + " Veteran";
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
        return "Damage: " + GetCurrentDamage() + "\nFire Rate: " + attackCooldown + "s\nRange: " + attackRange;
    }

    // update target tracking and attack cooldown
    void Update()
    {
        if (currentTarget != null && !IsTargetValid())
        {
            currentTarget = null;
        }

        if (currentTarget == null)
        {
            FindTarget();
        }

        if (currentTarget != null)
        {
            RotateTowardsTarget();
        }

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (cooldownTimer <= 0f && currentTarget != null)
        {
            Attack(currentTarget);
        }
    }

    // check if current target is alive and in range
    bool IsTargetValid()
    {
        if (currentTarget == null)
        {
            return false;
        }

        Health enemyHealth = currentTarget.GetComponent<Health>();

        if (enemyHealth != null && enemyHealth.isDead)
        {
            return false;
        }

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

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

    // attack target by spawning a projectile
    void Attack(GameObject target)
    {
        cooldownTimer = attackCooldown;

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
