using UnityEngine;

// panel showing inspected troop details and upgrade buttons
public class TroopPanel : MonoBehaviour
{
    public static TroopPanel instance; // static reference to troop panel

    // initialise instance on awake
    void Awake()
    {
        instance = this;
    }
}
