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

    // check inspected troop each frame and toggle panel
    void Update()
    {
        Tower tower = null;

        if (GridSelector.instance != null && !GridSelector.instance.isPlacing)
        {
            tower = GridSelector.instance.GetInspectedTower();
        }

        if (SkillTreeUI.instance != null && SkillTreeUI.instance.isOpen)
        {
            tower = null;
        }

        if (tower == null)
        {
            if (panel != null && panel.activeSelf)
            {
                panel.SetActive(false);
            }
            return;
        }

        if (panel != null && !panel.activeSelf)
        {
            panel.SetActive(true);
        }

        Refresh(tower);
    }

    // update panel content for the inspected troop
    void Refresh(Tower tower)
    {
        if (tower == null || SkillTree.instance == null)
        {
            return;
        }

        if (nameText != null)
        {
            nameText.text = tower.GetTroopName();
            nameText.color = SkillTree.instance.GetBranchColour(tower.branch);
        }

        if (levelText != null)
        {
            levelText.text = SkillTree.GetLevelName(tower.level);
        }

        if (statsText != null)
        {
            statsText.text = tower.GetStatsText();
        }

        int nextLevel = tower.level + 1;

        UpdateButton(physicalButton, physicalButtonText, tower, Branch.Physical, nextLevel);
        UpdateButton(mechanicButton, mechanicButtonText, tower, Branch.Mechanic, nextLevel);
        UpdateButton(magicButton, magicButtonText, tower, Branch.Magic, nextLevel);
        UpdateButton(imaginaryButton, imaginaryButtonText, tower, Branch.Imaginary, nextLevel);

        if (hintText != null)
        {
            if (nextLevel > 3)
            {
                hintText.text = "Max level reached";
            }
            else if (GridSelector.instance != null && GridSelector.instance.GetSelectedTower() != tower)
            {
                hintText.text = "Click troop to upgrade";
            }
            else
            {
                hintText.text = "Right click to deselect";
            }
        }
    }

    // update state and label of an upgrade button
    void UpdateButton(Button button, Text label, Tower tower, Branch branch, int nextLevel)
    {
        if (button == null || SkillTree.instance == null)
        {
            return;
        }

        bool show = nextLevel == 1 || (nextLevel > 1 && tower.branch == branch);
        button.gameObject.SetActive(show && nextLevel <= 3);

        if (!show || nextLevel > 3)
        {
            return;
        }

        string reason;
        bool canUpgrade = SkillTree.instance.CanUpgradeTroop(tower, branch, nextLevel, out reason);
        button.interactable = canUpgrade;

        if (label != null)
        {
            label.text = branch + " (" + reason + ")";
        }
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
