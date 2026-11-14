using UnityEngine;
using System.Collections.Generic;

public class RailBlastProjectile : MonoBehaviour
{
    public float damage; // set to 6x the firing tower's current damage
    public float explosionRadius = 2.5f; // replaced with the firing tower's splash radius
    private List<Health> hitEnemies = new List<Health>();
    private bool exploded;

    public void HitEnemy(Health enemy, Vector3 hitPosition)
    {
        if (exploded || enemy == null || enemy.isDead || !enemy.CompareTag("Enemy") || hitEnemies.Contains(enemy))
        {
            return;
        }

        hitEnemies.Add(enemy);
        enemy.TakeDamage(damage);
        if (hitEnemies.Count < 3)
        {
            return;
        }

        // The third enemy receives the beam hit, then the blast affects nearby enemies.
        exploded = true;
        List<Health> explosionEnemies = new List<Health>();
        foreach (Collider hit in Physics.OverlapSphere(hitPosition, explosionRadius))
        {
            Health nearby = hit.GetComponentInParent<Health>();
            if (nearby == null || nearby.isDead || !nearby.CompareTag("Enemy") || explosionEnemies.Contains(nearby))
            {
                continue;
            }

            explosionEnemies.Add(nearby);
            nearby.TakeDamage(damage * 0.5f);
        }

        Destroy(gameObject);
    }
}
