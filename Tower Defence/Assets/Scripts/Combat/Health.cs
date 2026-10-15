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
}
