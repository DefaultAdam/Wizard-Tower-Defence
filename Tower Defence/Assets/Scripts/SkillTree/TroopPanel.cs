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

    public Button physicalButton; // button to upgrade to physical branch
    public Button mechanicButton; // button to upgrade to mechanic branch
    public Button magicButton; // button to upgrade to magic branch
    public Button imaginaryButton; // button to upgrade to imaginary branch

    public Text physicalButtonText; // text display for physical upgrade button
    public Text mechanicButtonText; // text display for mechanic upgrade button
    public Text magicButtonText; // text display for magic upgrade button
    public Text imaginaryButtonText; // text display for imaginary upgrade button

    // initialise instance on awake
    void Awake()
    {
        instance = this;
    }
}
