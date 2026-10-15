using UnityEngine;

// projectile script for tower attacks
public class Projectile : MonoBehaviour
{
    public float speed = 10f; // projectile movement speed
    public float damage = 20f; // damage dealt on hit

    private Transform target; // target to follow

    // set target for projectile to follow
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // move toward target every frame
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            HitTarget();
        }
    }

    // hit target and deal damage
    void HitTarget()
    {
        Health targetHealth = target.GetComponent<Health>();

        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
