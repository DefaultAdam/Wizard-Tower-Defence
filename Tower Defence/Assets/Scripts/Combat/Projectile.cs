using UnityEngine;

// projectile script for tower attacks
public class Projectile : MonoBehaviour
{
    public float speed = 10f; // projectile movement speed
    public float damage = 20f; // damage dealt on hit
    public float aoeRadius = 0f;
    public float aoeDamagePercent = 0.5f;
    public int splashStoneCount = 8;
    public float splashEffectDuration = 0.35f;

    private Transform target; // target to follow
    private GameObject[] splashStones;
    private Vector3 splashCenter;
    private float splashTimer;
    private bool splashing;

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
        if (splashing)
        {
            UpdateSplashEffect();
            return;
        }

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
            ShowSplashEffect(hitPosition);
            return;
        }

        Destroy(gameObject);
    }

    void ShowSplashEffect(Vector3 center)
    {
        Collider projectileCollider = GetComponent<Collider>();

        if (projectileCollider != null)
        {
            projectileCollider.enabled = false;
        }

        Renderer stoneRenderer = GetComponentInChildren<Renderer>();

        splashCenter = center + new Vector3(0f, 0.3f, 0f);
        splashStones = new GameObject[splashStoneCount];

        for (int i = 0; i < splashStoneCount; i++)
        {
            GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(stone.GetComponent<Collider>());

            stone.transform.position = splashCenter;
            stone.transform.localScale = transform.localScale;
            stone.transform.parent = transform;

            if (stoneRenderer != null)
            {
                stone.GetComponent<Renderer>().sharedMaterial = stoneRenderer.sharedMaterial;
            }

            splashStones[i] = stone;
        }

        if (stoneRenderer != null)
        {
            stoneRenderer.enabled = false;
        }

        splashTimer = 0f;
        splashing = true;
    }

    void UpdateSplashEffect()
    {
        splashTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(splashTimer / splashEffectDuration);

        for (int i = 0; i < splashStones.Length; i++)
        {
            if (splashStones[i] != null)
            {
                float angle = i * Mathf.PI * 2f / splashStones.Length;
                Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
                splashStones[i].transform.position = splashCenter + direction * (aoeRadius * progress);
            }
        }

        if (progress >= 1f)
        {
            for (int i = 0; i < splashStones.Length; i++)
            {
                if (splashStones[i] != null)
                {
                    Destroy(splashStones[i]);
                }
            }

            Destroy(gameObject);
        }
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
