using UnityEngine;
using UnityEngine.Events;

// enemy script for rewards and death events
public class Enemy : MonoBehaviour
{
    public int spReward = 10; // skill points rewarded on kill
    public UnityEvent onKilled; // event triggered when killed

    public float distanceTravelled = 0f; // distance travelled along path

    private Health health; // reference to health component
    private Vector3 lastPosition; // previous position to calculate distance travelled

    // setup starting values and listen for death on start
    void Start()
    {
        lastPosition = transform.position;
        health = GetComponent<Health>();

        if (health != null)
        {
            health.onDeath.AddListener(OnDeath);
        }
    }

    // update distance travelled along path
    void Update()
    {
        float distanceThisFrame = Vector3.Distance(transform.position, lastPosition);
        distanceTravelled += distanceThisFrame;
        lastPosition = transform.position;
    }

    // reward sp and dispatch kill event when enemy dies
    void OnDeath()
    {
        if (HUD.instance != null)
        {
            HUD.instance.AddSP(spReward);
        }

        if (onKilled != null)
        {
            onKilled.Invoke();
        }
    }
}
