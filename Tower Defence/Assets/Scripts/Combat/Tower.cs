using UnityEngine;

// tower combat and attack script
public class Tower : MonoBehaviour
{
    public float attackRange = 5f; // attack range distance
    public float attackCooldown = 1f; // seconds between attacks
    public float damage = 20f; // damage dealt per attack

    private float cooldownTimer = 0f; // timer to track attack cooldown
    private GameObject currentTarget; // current target enemy

    // update cooldown timer every frame
    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (cooldownTimer <= 0f)
        {
            FindTarget();
        }
    }

    // find nearest enemy in range
    void FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = attackRange;
        currentTarget = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                currentTarget = enemy;
            }
        }
    }
}
