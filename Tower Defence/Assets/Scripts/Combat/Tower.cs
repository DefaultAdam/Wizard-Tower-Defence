using UnityEngine;

// targeting priority options
public enum TargetPriority
{
    First,
    Closest,
    Strongest,
    Weakest
}

// tower combat and attack script
public class Tower : MonoBehaviour
{
    public float attackRange = 6f; // attack range distance
    public float attackCooldown = 1f; // seconds between attacks
    public float damage = 25f; // damage dealt per attack

    public float damagePerWave = 5f; // extra damage gained each round
    public float aoeRadius = 0f;
    public float aoeDamagePercent = 0.5f;
    public TargetPriority targetPriority = TargetPriority.Closest; // targeting priority criteria

    public GameObject projectilePrefab; // projectile prefab to shoot
    public Transform firePoint; // spawn point for projectiles
    public Transform towerHead; // rotating part of the tower
    public float rotationSpeed = 10f; // rotation speed towards target

    private float cooldownTimer = 0f; // timer to track attack cooldown
    private GameObject currentTarget; // current target enemy

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
                float totalDamage = damage;

                if (HUD.instance != null && HUD.instance.currentWave > 1)
                {
                    totalDamage += (HUD.instance.currentWave - 1) * damagePerWave;
                }

                projectile.damage = totalDamage;
                projectile.aoeRadius = aoeRadius;
                projectile.aoeDamagePercent = aoeDamagePercent;
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
