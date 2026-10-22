using UnityEngine;
using UnityEngine.UI;

// panel showing inspected troop details and upgrade buttons
public class TroopPanel : MonoBehaviour
{
    public static TroopPanel instance; // static reference to troop panel

    public GameObject panel; // root panel game object
    public Text nameText; // troop name text display
    public Text levelText; // troop level text display
    public Text statsText; // troop combat stats text display
    public Text hintText; // interaction hint text display

    // initialise instance on awake
    void Awake()
    {
        instance = this;
    }
}
