using UnityEngine;
using System.Collections.Generic;

public class RailBlastProjectile : MonoBehaviour
{
    public float damage; // set to 6x the firing tower's current damage
    public float explosionRadius = 2.5f; // replaced with the firing tower's splash radius
    public float speed = 20f;
    public float maxDistance = 10f; // replaced with the firing tower's attack range
    public float beamWidth = 0.5f; // replaced with half a grid tile
    private List<Health> hitEnemies = new List<Health>();
    private bool exploded;
    private float travelled;

    private void Update()
    {
        if (exploded || Time.deltaTime <= 0f)
        {
            return;
        }

        float distance = Mathf.Min(speed * Time.deltaTime, maxDistance - travelled);
        if (distance <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, beamWidth * 0.5f, transform.forward, distance, ~0, QueryTriggerInteraction.Collide);
        System.Array.Sort(hits, (first, second) => first.distance.CompareTo(second.distance));
        foreach (RaycastHit hit in hits)
        {
            HitEnemy(hit.collider.GetComponentInParent<Health>(), hit.point);
            if (exploded)
            {
                return;
            }
        }

        transform.position += transform.forward * distance;
        travelled += distance;
        if (travelled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

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
