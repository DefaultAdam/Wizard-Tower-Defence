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

    public Button physicalApprenticeButton; // button to unlock physical apprentice
    public Button mechanicApprenticeButton; // button to unlock mechanic apprentice
    public Button magicApprenticeButton; // button to unlock magic apprentice
    public Button imaginaryApprenticeButton; // button to unlock imaginary apprentice

    public Text physicalApprenticeText; // text display for physical apprentice button
    public Text mechanicApprenticeText; // text display for mechanic apprentice button
    public Text magicApprenticeText; // text display for magic apprentice button
    public Text imaginaryApprenticeText; // text display for imaginary apprentice button

    public Button physicalVeteranButton; // button to unlock physical veteran
    public Button mechanicVeteranButton; // button to unlock mechanic veteran
    public Button magicVeteranButton; // button to unlock magic veteran
    public Button imaginaryVeteranButton; // button to unlock imaginary veteran

    public Text physicalVeteranText; // text display for physical veteran button
    public Text mechanicVeteranText; // text display for mechanic veteran button
    public Text magicVeteranText; // text display for magic veteran button
    public Text imaginaryVeteranText; // text display for imaginary veteran button

    // initialise instance on awake
    void Awake()
    {
        instance = this;
    }

    // set up button click listeners on start
    void Start()
    {
        if (openButton != null)
        {
            openButton.onClick.AddListener(Toggle);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }

        if (window != null)
        {
            window.SetActive(false);
        }
    }

    // handle toggle key and refresh open window
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            Toggle();
        }

        if (isOpen)
        {
            RefreshWindow();
        }
    }

    // open or close skill tree window
    public void Toggle()
    {
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    // open skill tree window and clear placement or selection
    public void Open()
    {
        isOpen = true;

        if (window != null)
        {
            window.SetActive(true);
        }

        if (GridSelector.instance != null)
        {
            GridSelector.instance.CancelPlacement();
            GridSelector.instance.ClearSelection();
        }

        RefreshWindow();
    }

    // close skill tree window
    public void Close()
    {
        isOpen = false;

        if (window != null)
        {
            window.SetActive(false);
        }
    }

    // update window display elements
    public void RefreshWindow()
    {
        if (SkillTree.instance == null)
        {
            return;
        }

        if (spText != null)
        {
            spText.text = "SP: " + SkillTree.instance.GetCurrentSP();
        }
    }
}
