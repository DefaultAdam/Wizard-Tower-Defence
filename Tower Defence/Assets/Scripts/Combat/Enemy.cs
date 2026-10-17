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
