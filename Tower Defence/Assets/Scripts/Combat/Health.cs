using UnityEngine;
using UnityEngine.Events;

// health script for enemies
public class Health : MonoBehaviour
{
    public float maxHealth = 100f; // total health
    public float currentHealth; // current health
    public bool isDead = false; // check if dead

    public UnityEvent onDeath; // event when this object dies

    // set health at start
    void Start()
    {
        currentHealth = maxHealth;
    }

    // take damage from an attack
    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // destroy object when health reaches zero
    void Die()
    {
        isDead = true;

        if (onDeath != null)
        {
            onDeath.Invoke();
        }

        Destroy(gameObject);
    }
}
