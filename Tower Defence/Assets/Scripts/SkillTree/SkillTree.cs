using UnityEngine;

// skill tree manager
public class SkillTree : MonoBehaviour
{
    public static SkillTree instance; // static reference to skill tree

    // initialise instance on awake
    void Awake()
    {
        instance = this;
    }
}
