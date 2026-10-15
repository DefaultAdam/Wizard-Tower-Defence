using UnityEngine;

// tower combat and attack script
public class Tower : MonoBehaviour
{
    public float attackRange = 5f; // attack range distance
    public float attackCooldown = 1f; // seconds between attacks
    public float damage = 20f; // damage dealt per attack

    private float cooldownTimer = 0f; // timer to track attack cooldown

    // update cooldown timer every frame
    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
}
