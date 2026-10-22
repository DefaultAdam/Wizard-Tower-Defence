using UnityEngine;

// skill tree manager
public class SkillTree : MonoBehaviour
{
    public static SkillTree instance; // static reference to skill tree

    public int apprenticeUnlockSP = 150; // sp milestone needed to unlock level 1 apprentices
    public int apprenticePathCost = 25; // sp cost to unlock an apprentice path in the skill tree
    public int apprenticeUpgradeCost = 25; // fee to upgrade recruit to level 1 apprentice
    public int veteranUnlockSP = 250; // sp milestone needed to unlock level 2 veterans
    public int veteranPathCost = 50; // sp cost to unlock a veteran path in the skill tree
    public int veteranUpgradeCost = 50; // fee to upgrade apprentice to level 2 veteran

    // initialise instance on awake
    void Awake()
    {
        instance = this;
    }
}
