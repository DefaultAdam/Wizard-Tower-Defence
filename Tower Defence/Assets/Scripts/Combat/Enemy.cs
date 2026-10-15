using UnityEngine;
using UnityEngine.Events;

// enemy script for rewards and death events
public class Enemy : MonoBehaviour
{
    public int spReward = 10; // skill points rewarded on kill
    public UnityEvent onKilled; // event triggered when killed

    private Health health; // reference to health component

    // listen for death on start
    void Start()
    {
        health = GetComponent<Health>();

        if (health != null)
        {
            health.onDeath.AddListener(OnDeath);
        }
    }

    // dispatch kill event when enemy dies
    void OnDeath()
    {
        if (onKilled != null)
        {
            onKilled.Invoke();
        }
    }
}
