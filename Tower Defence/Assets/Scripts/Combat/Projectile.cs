using UnityEngine;

// projectile script for tower attacks
public class Projectile : MonoBehaviour
{
    public float speed = 10f; // projectile movement speed
    public float damage = 20f; // damage dealt on hit
    public float aoeRadius = 0f;
    public float aoeDamagePercent = 0.5f;

    private Transform target; // target to follow

    // set target for projectile to follow
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (target != null)
        {
            transform.LookAt(target);
        }
    }

    // move toward target and face trajectory every frame
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.LookAt(target);
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            HitTarget();
        }
    }

    // hit target and deal damage
    void HitTarget()
    {
        Vector3 hitPosition = target.position;
        Health targetHealth = target.GetComponent<Health>();

        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }

        if (aoeRadius > 0f)
        {
            DealAreaDamage(hitPosition);
        }

        Destroy(gameObject);
    }

    void DealAreaDamage(Vector3 center)
    {
        Collider[] colliders = Physics.OverlapSphere(center, aoeRadius);

        foreach (Collider hitCollider in colliders)
        {
            if (hitCollider == null || !hitCollider.CompareTag("Enemy"))
            {
                continue;
            }

            if (hitCollider.transform == target)
            {
                continue;
            }

            Health enemyHealth = hitCollider.GetComponent<Health>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage * aoeDamagePercent);
            }
        }
    }
}
