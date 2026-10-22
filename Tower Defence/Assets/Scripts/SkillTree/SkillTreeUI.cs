using UnityEngine;
using UnityEngine.UI;

// skill tree window ui controller
public class SkillTreeUI : MonoBehaviour
{
    public static SkillTreeUI instance; // static reference to skill tree ui

    public GameObject window; // full screen skill tree window root
    public Text spText; // window sp pool display
    public Button openButton; // button to open skill tree window
    public Button closeButton; // button to close skill tree window
    public KeyCode toggleKey = KeyCode.Tab; // keyboard shortcut to toggle skill tree window
    public bool isOpen = false; // current window open state

    // initialise instance on awake
    void Awake()
    {
        instance = this;
    }
}
