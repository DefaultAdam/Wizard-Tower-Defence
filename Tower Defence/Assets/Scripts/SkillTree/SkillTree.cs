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
}
