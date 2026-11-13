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
    public Button benchButton; // button to bench inspected recruit
    public Text benchButtonText; // text display for bench button
    private Button fuseButton; // shares the bench button position, shown only for Masters

    // initialise instance on awake
    void Awake()
    {
        instance = this;
    }

    // check inspected troop each frame and toggle panel
    void Update()
    {
        if (GridSelector.instance != null && GridSelector.instance.isPlacing)
        {
            HideUpgradeButtons();
            return;
        }

        Tower tower = null;

        if (GridSelector.instance != null)
        {
            tower = GridSelector.instance.GetInspectedTower();
        }

        if ((SkillTreeUI.instance != null && SkillTreeUI.instance.isOpen) ||
            (ReserveBenchUI.instance != null && ReserveBenchUI.instance.isOpen))
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

    // hide upgrade buttons during placement preview
    void HideUpgradeButtons()
    {
        if (fuseButton != null)
        {
            fuseButton.gameObject.SetActive(false);
        }

        if (physicalButton != null)
        {
            physicalButton.gameObject.SetActive(false);
        }

        if (mechanicButton != null)
        {
            mechanicButton.gameObject.SetActive(false);
        }

        if (magicButton != null)
        {
            magicButton.gameObject.SetActive(false);
        }

        if (imaginaryButton != null)
        {
            imaginaryButton.gameObject.SetActive(false);
        }

        if (benchButton != null)
        {
            benchButton.gameObject.SetActive(false);
        }

        if (hintText != null)
        {
            hintText.gameObject.SetActive(false);
        }
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

        if (fuseButton != null)
        {
            fuseButton.gameObject.SetActive(tower.level == 3 && !tower.isSuper && tower.branch != Branch.None);
        }

        UpdateButton(physicalButton, physicalButtonText, tower, Branch.Physical, nextLevel);
        UpdateButton(mechanicButton, mechanicButtonText, tower, Branch.Mechanic, nextLevel);
        UpdateButton(magicButton, magicButtonText, tower, Branch.Magic, nextLevel);
        UpdateButton(imaginaryButton, imaginaryButtonText, tower, Branch.Imaginary, nextLevel);

        if (benchButton != null)
        {
            if (tower.level == 0 && ReserveBench.instance != null)
            {
                benchButton.gameObject.SetActive(true);
                bool canBench = ReserveBench.instance.HasCapacity();
                benchButton.interactable = canBench;
                if (benchButtonText != null)
                {
                    benchButtonText.text = canBench ? "Send to Bench [X]" : "Bench Full (5/5)";
                }
            }
            else
            {
                benchButton.gameObject.SetActive(false);
            }
        }

        if (hintText != null)
        {
            if (GridSelector.instance != null && GridSelector.instance.isSelectingFusion)
            {
                hintText.text = "Fusion: " + GridSelector.instance.FusionSelectionCount + "/3 selected. Click to toggle; right click or Escape to cancel.";
            }
            else if (nextLevel > 3)
            {
                hintText.text = "Max level reached";
            }
            else if (GridSelector.instance != null && GridSelector.instance.GetSelectedTower() != tower)
            {
                hintText.text = tower.level == 0 ? "Click troop to upgrade (or 'X' to bench)" : "Click troop to upgrade";
            }
            else
            {
                hintText.text = tower.level == 0 ? "Right click to deselect (or 'X' to bench)" : "Right click to deselect";
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
        if (benchButton == null && panel != null)
        {
            Transform found = panel.transform.Find("Bench Button");
            if (found != null)
            {
                benchButton = found.GetComponent<Button>();
                benchButtonText = found.GetComponentInChildren<Text>();
            }
        }

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

        if (benchButton != null)
        {
            benchButton.onClick.AddListener(OnBenchButtonClicked);
            fuseButton = Instantiate(benchButton, benchButton.transform.parent);
            fuseButton.gameObject.name = "Fuse Button";
            fuseButton.onClick = new Button.ButtonClickedEvent();
            fuseButton.onClick.AddListener(OnFuseButtonClicked);
            fuseButton.GetComponentInChildren<Text>().text = "Fuse";
            fuseButton.gameObject.SetActive(false);
        }
    }

    void OnFuseButtonClicked()
    {
        if (GridSelector.instance != null)
        {
            GridSelector.instance.StartFusionSelection();
        }
    }

    // handle bench button click to send inspected recruit to reserve bench
    public void OnBenchButtonClicked()
    {
        Tower tower = null;
        if (GridSelector.instance != null)
        {
            tower = GridSelector.instance.GetInspectedTower();
        }

        if (tower != null && tower.level == 0 && GridSelector.instance != null)
        {
            GridSelector.instance.BenchTower(tower);
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
