using UnityEngine;

// skill tree manager
public class SkillTree : MonoBehaviour
{
    public static SkillTree instance; // static reference to skill tree

    public static readonly Branch[] branches = { Branch.Physical, Branch.Mechanic, Branch.Magic, Branch.Imaginary }; // all specialisation branches

    public int apprenticeUnlockSP = 250; // sp milestone needed to unlock level 1 apprentices
    public int apprenticeUpgradeCost = 100; // fee to upgrade recruit to level 1 apprentice
    public int veteranUnlockSP = 350; // sp milestone needed to unlock level 2 veterans
    public int veteranUpgradeCost = 150; // fee to upgrade apprentice to level 2 veteran
    public int masterUnlockSP = 400; // sp milestone needed to unlock level 3 masters
    public int masterUpgradeCost = 200; // fee to upgrade veteran to level 3 master

    public Color recruitColour = Color.white; // colour for level 0 recruits
    public Color physicalColour = new Color(0.78f, 0.6f, 0.42f); // light brown
    public Color mechanicColour = new Color(0.45f, 0.56f, 0.68f); // grey blue
    public Color magicColour = new Color(0.6f, 0.3f, 0.85f); // purple
    public Color imaginaryColour = new Color(1f, 0.85f, 0.2f); // yellow

    // initialise instance on awake
    void Awake()
    {
        instance = this;
    }

    // get current sp from hud
    public int GetCurrentSP()
    {
        if (HUD.instance == null)
        {
            return 0;
        }

        return HUD.instance.currentSP;
    }

    // get sp milestone needed for a level
    public int GetUnlockSP(int level)
    {
        if (level == 1)
        {
            return apprenticeUnlockSP;
        }

        if (level == 2)
        {
            return veteranUnlockSP;
        }

        if (level == 3)
        {
            return masterUnlockSP;
        }

        return 0;
    }

    // get fee to upgrade a troop to a level
    public int GetUpgradeCost(int level)
    {
        if (level == 1)
        {
            return apprenticeUpgradeCost;
        }

        if (level == 2)
        {
            return veteranUpgradeCost;
        }

        if (level == 3)
        {
            return masterUpgradeCost;
        }

        return 0;
    }

    // get display colour for a branch
    public Color GetBranchColour(Branch branch)
    {
        if (branch == Branch.Physical)
        {
            return physicalColour;
        }

        if (branch == Branch.Mechanic)
        {
            return mechanicColour;
        }

        if (branch == Branch.Magic)
        {
            return magicColour;
        }

        if (branch == Branch.Imaginary)
        {
            return imaginaryColour;
        }

        return recruitColour;
    }

    // check if a troop can be upgraded to a branch tier and give reason if not
    public bool CanUpgradeTroop(Tower tower, Branch branch, int targetLevel, out string reason)
    {
        reason = "";

        if (tower == null || tower.level != targetLevel - 1)
        {
            reason = "Unavailable";
            return false;
        }

        if (targetLevel >= 2 && tower.branch != branch)
        {
            reason = "Wrong path";
            return false;
        }

        if (GetCurrentSP() < GetUnlockSP(targetLevel))
        {
            reason = "Needs " + GetUnlockSP(targetLevel) + " SP milestone";
            return false;
        }

        if (GetCurrentSP() < GetUpgradeCost(targetLevel))
        {
            reason = "Not enough SP";
            return false;
        }

        reason = GetUpgradeCost(targetLevel) + " SP";
        return true;
    }

    // spend sp to upgrade a placed troop
    public bool TryUpgradeTroop(Tower tower, Branch branch, int targetLevel)
    {
        string reason;

        if (!CanUpgradeTroop(tower, branch, targetLevel, out reason))
        {
            return false;
        }

        int cost = GetUpgradeCost(targetLevel);

        if (HUD.instance != null)
        {
            HUD.instance.DeductSP(cost);
            HUD.instance.ShowDeduction(cost);
        }

        if (targetLevel == 1)
        {
            tower.UpgradeToApprentice(branch);
        }
        else if (targetLevel == 2)
        {
            Tower veteranTemplate = null;

            if (GridSelector.instance != null && GridSelector.instance.veteranPrefab != null)
            {
                veteranTemplate = GridSelector.instance.veteranPrefab.GetComponent<Tower>();
            }

            tower.UpgradeToVeteran(veteranTemplate);
        }
        else if (targetLevel == 3)
        {
            tower.UpgradeToMaster(null);
        }

        return true;
    }

    // get display text for a level tier
    public static string GetLevelName(int level)
    {
        if (level == 1)
        {
            return "Level 1 Apprentice";
        }

        if (level == 2)
        {
            return "Level 2 Veteran";
        }

        if (level == 3)
        {
            return "Level 3 Master";
        }

        return "Level 0 Recruit";
    }
}
