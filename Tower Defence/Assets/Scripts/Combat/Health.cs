using UnityEngine;

// health script for enemies
public class Health : MonoBehaviour
{
    public float maxHealth = 100f; // total health
    public float currentHealth; // current health

    // set health at start
    void Start()
    {
        currentHealth = maxHealth;
    }

    // take damage from an attack
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // destroy object when health reaches zero
    void Die()
    {
        Destroy(gameObject);
    }
}
