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

    // set up button listeners on start
    void Start()
    {
        if (physicalButton != null)
        {
            physicalButton.onClick.AddListener(UpgradeToPhysical);
        }

        if (mechanicButton != null)
        {
            mechanicButton.onClick.AddListener(UpgradeToMechanic);
        }

        if (magicButton != null)
        {
            magicButton.onClick.AddListener(UpgradeToMagic);
        }

        if (imaginaryButton != null)
        {
            imaginaryButton.onClick.AddListener(UpgradeToImaginary);
        }
    }

    // upgrade to physical branch
    public void UpgradeToPhysical()
    {
        OnUpgradeClicked(Branch.Physical);
    }

    // upgrade to mechanic branch
    public void UpgradeToMechanic()
    {
        OnUpgradeClicked(Branch.Mechanic);
    }

    // upgrade to magic branch
    public void UpgradeToMagic()
    {
        OnUpgradeClicked(Branch.Magic);
    }

    // upgrade to imaginary branch
    public void UpgradeToImaginary()
    {
        OnUpgradeClicked(Branch.Imaginary);
    }

    // upgrade selected troop on clicked branch
    void OnUpgradeClicked(Branch branch)
    {
        if (GridSelector.instance == null || SkillTree.instance == null)
        {
            return;
        }

        Tower tower = GridSelector.instance.GetSelectedTower();

        if (tower == null)
        {
            return;
        }

        SkillTree.instance.TryUpgradeTroop(tower, branch, tower.level + 1);
    }
}
