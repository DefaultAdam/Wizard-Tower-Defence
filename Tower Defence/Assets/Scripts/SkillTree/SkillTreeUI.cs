using UnityEngine;

// skill tree window ui controller
public class SkillTreeUI : MonoBehaviour
{
    public static SkillTreeUI instance; // static reference to skill tree ui

    // initialise instance on awake
    void Awake()
    {
        instance = this;
    }
}
