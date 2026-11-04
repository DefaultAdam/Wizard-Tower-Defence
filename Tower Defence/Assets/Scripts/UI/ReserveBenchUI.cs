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

    public Button[] slotButtons = new Button[5]; // action buttons for five bench slots
    public Text[] slotButtonTexts = new Text[5]; // text labels on slot action buttons
    public Text capacityText; // text display for benched units count
    public Text passiveText; // text display for total passive sp gained

    private float currentX = 520f; // current animated position
    private float lastToggleTime = -1f; // debounce timer for toggling menu

    // initialise instance on awake
    void Awake()
    {
        instance = this;
    }

    // set up button listeners on start
    void Start()
    {
        if (openMenuButton != null)
        {
            openMenuButton.onClick.AddListener(Toggle);
        }

        if (closeMenuButton != null)
        {
            closeMenuButton.onClick.AddListener(Close);
        }

        currentX = panelWidth + 60f;
        if (panelRect != null)
        {
            panelRect.anchoredPosition = new Vector2(currentX, 0f);
        }

        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
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

        UpdateSlideAnimation();
    }

    // update drawer slide position towards target
    void UpdateSlideAnimation()
    {
        float targetX = isOpen ? 0f : (panelWidth + 60f);

        currentX = Mathf.Lerp(currentX, targetX, Time.unscaledDeltaTime * slideSpeed);

        if (Mathf.Abs(currentX - targetX) < 0.5f)
        {
            currentX = targetX;

            if (!isOpen && menuPanel != null && menuPanel.activeSelf)
            {
                menuPanel.SetActive(false);
            }
        }

        if (panelRect != null)
        {
            panelRect.anchoredPosition = new Vector2(currentX, 0f);
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

        if (openMenuButton != null)
        {
            openMenuButton.gameObject.SetActive(false);
        }

        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
        }
    }

    // slide closed reserve bench menu drawer
    public void Close()
    {
        isOpen = false;

        if (openMenuButton != null)
        {
            openMenuButton.gameObject.SetActive(true);
        }
    }
}
