using UnityEngine;

// game loop manager for game flow
public class GameManager : MonoBehaviour
{
    public static GameManager instance; // static reference to game manager
    public bool isPaused = false; // current pause state

    // set instance on awake
    void Awake()
    {
        instance = this;
    }
}
