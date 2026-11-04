using UnityEngine;
using UnityEngine.UI;

// sliding reserve bench menu controller and visual interface
public class ReserveBenchUI : MonoBehaviour
{
    public static ReserveBenchUI instance; // static reference to reserve bench ui

    public GameObject menuPanel; // drawer panel on the right
    public RectTransform panelRect; // rect transform for sliding drawer
    public Button openMenuButton; // button to toggle reserve bench menu
    public Text openButtonText; // text display on open button
    public Button closeMenuButton; // close button inside header
    public KeyCode toggleKey = KeyCode.B; // hotkey to toggle menu
    public bool isOpen = false; // whether drawer is currently open
    public float slideSpeed = 14f; // sliding animation speed
    public float panelWidth = 460f; // width of sliding drawer

    private float lastToggleTime = -1f; // debounce timer for toggling menu

    // initialise instance on awake
    void Awake()
    {
        instance = this;
    }

    // handle hotkey input each frame
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            Toggle();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
        {
            Close();
        }
    }

    // toggle sliding menu open or closed with debounce check
    public void Toggle()
    {
        if (Time.unscaledTime - lastToggleTime < 0.2f)
        {
            return;
        }
        lastToggleTime = Time.unscaledTime;

        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    // slide open reserve bench menu drawer
    public void Open()
    {
        isOpen = true;

        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
        }
    }

    // slide closed reserve bench menu drawer
    public void Close()
    {
        isOpen = false;

        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }
}
