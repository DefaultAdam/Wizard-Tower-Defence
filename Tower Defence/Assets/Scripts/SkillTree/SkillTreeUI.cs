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
    }
}
