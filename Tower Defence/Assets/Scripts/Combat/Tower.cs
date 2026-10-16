using UnityEngine;

// tower combat and attack script
public class Tower : MonoBehaviour
{
    public float attackRange = 5f; // attack range distance
    public float attackCooldown = 1f; // seconds between attacks
    public float damage = 20f; // damage dealt per attack

    public GameObject projectilePrefab; // projectile prefab to shoot
    public Transform firePoint; // spawn point for projectiles

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

    // find nearest enemy in range
    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = attackRange;
        currentTarget = null;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            Health enemyHealth = enemy.GetComponent<Health>();

            if (enemyHealth != null && enemyHealth.isDead)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                currentTarget = enemy;
            }
        }
    }

    // attack target by spawning a projectile
    void Attack(GameObject target)
    {
        cooldownTimer = attackCooldown;

        if (projectilePrefab != null)
        {
            Vector3 spawnPosition = transform.position;

            if (firePoint != null)
            {
                spawnPosition = firePoint.position;
            }

            GameObject projectileObject = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            Projectile projectile = projectileObject.GetComponent<Projectile>();

            if (projectile != null)
            {
                projectile.damage = damage;
                projectile.SetTarget(target.transform);
            }
        }
    }
}
