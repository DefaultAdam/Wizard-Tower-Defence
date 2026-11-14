using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
    private Text[] slotTitleTexts = new Text[5]; // unit names above the action buttons
    public Text capacityText; // text display for benched units count
    public Text passiveText; // text display for total passive sp gained
    private bool isSelectingFusion;
    private List<int> fusionSlots = new List<int>();
    private Text fuseButtonText;
    private string fusionMessage = "";

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
            Button fuseButton = Instantiate(closeMenuButton, closeMenuButton.transform.parent);
            fuseButton.gameObject.name = "Bench Fuse Button";
            fuseButton.onClick = new Button.ButtonClickedEvent();
            fuseButton.onClick.AddListener(StartFusionSelection);
            RectTransform rect = fuseButton.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(-60f, -12f);
            rect.sizeDelta = new Vector2(125f, 36f);
            fuseButtonText = fuseButton.GetComponentInChildren<Text>();
            fuseButtonText.fontSize = 18;
            Transform title = closeMenuButton.transform.parent.Find("TitleText");
            if (title != null)
            {
                title.GetComponent<RectTransform>().sizeDelta = new Vector2(-200f, 42f);
            }
        }

        currentX = panelWidth + 60f;
        if (panelRect != null)
        {
            panelRect.anchoredPosition = new Vector2(currentX, 0f);
        }

        FindSlotReferences();

        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }

        Refresh();
    }

    // find slot buttons and counters from panel if not assigned
    void FindSlotReferences()
    {
        if (menuPanel == null)
        {
            return;
        }

        if (capacityText == null)
        {
            Transform cap = menuPanel.transform.Find("Footer/CapacityText");
            if (cap != null)
            {
                capacityText = cap.GetComponent<Text>();
            }
        }

        if (passiveText == null)
        {
            Transform pas = menuPanel.transform.Find("Footer/TotalPassiveText");
            if (pas != null)
            {
                passiveText = pas.GetComponent<Text>();
            }
        }

        Transform container = menuPanel.transform.Find("SlotsContainer");
        if (container != null)
        {
            for (int i = 0; i < 5; i++)
            {
                Transform slot = container.Find("Slot_" + i);
                if (slot != null)
                {
                    Transform title = slot.Find("TitleText");
                    if (title != null)
                    {
                        slotTitleTexts[i] = title.GetComponent<Text>();
                    }

                    if (slotButtons[i] == null)
                    {
                        Transform btn = slot.Find("ActionButton");
                        if (btn != null)
                        {
                            slotButtons[i] = btn.GetComponent<Button>();
                        }
                    }

                    if (slotButtonTexts[i] == null && slotButtons[i] != null)
                    {
                        Transform txt = slotButtons[i].transform.Find("ActionText");
                        if (txt != null)
                        {
                            slotButtonTexts[i] = txt.GetComponent<Text>();
                        }
                    }
                }
            }
        }

        if (slotButtons[0] != null) slotButtons[0].onClick.AddListener(ClickSlot0);
        if (slotButtons[1] != null) slotButtons[1].onClick.AddListener(ClickSlot1);
        if (slotButtons[2] != null) slotButtons[2].onClick.AddListener(ClickSlot2);
        if (slotButtons[3] != null) slotButtons[3].onClick.AddListener(ClickSlot3);
        if (slotButtons[4] != null) slotButtons[4].onClick.AddListener(ClickSlot4);
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

        if (isOpen)
        {
            Refresh();
        }
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

        if (SkillTreeUI.instance != null && SkillTreeUI.instance.isOpen)
        {
            SkillTreeUI.instance.Close();
        }

        if (GridSelector.instance != null)
        {
            GridSelector.instance.CancelPlacement();
            GridSelector.instance.ClearSelection();
        }

        Refresh();
    }

    // slide closed reserve bench menu drawer
    public void Close()
    {
        fusionSlots.Clear();
        fusionMessage = "";
        isSelectingFusion = false;
        isOpen = false;

        if (openMenuButton != null)
        {
            openMenuButton.gameObject.SetActive(true);
        }
    }

    private void StartFusionSelection()
    {
        if (ReserveBench.instance == null)
        {
            return;
        }

        if (!isSelectingFusion)
        {
            isSelectingFusion = true;
            fusionMessage = "Choose 3 matching Level 3 units.";
        }
        else if (fusionSlots.Count != 3)
        {
            fusionMessage = "Select three units before confirming fusion.";
        }
        else
        {
            string reason;
            if (ReserveBench.instance.TryFuseSlots(fusionSlots[0], fusionSlots[1], fusionSlots[2], out reason))
            {
                fusionSlots.Clear();
                isSelectingFusion = false;
                fusionMessage = "Fusion complete.";
            }
            else
            {
                fusionMessage = reason;
            }
        }

        Refresh();
    }

    // refresh drawer counters and slot buttons
    public void Refresh()
    {
        if (menuPanel == null)
        {
            return;
        }

        int benched = 0;
        int maxBench = 5;
        int totalPassive = 0;

        if (ReserveBench.instance != null)
        {
            benched = ReserveBench.instance.benchedUnits;
            maxBench = ReserveBench.instance.maxBenchUnits;
            totalPassive = ReserveBench.instance.totalPassiveSPEarned;
        }

        if (capacityText != null)
        {
            capacityText.text = "Benched: " + benched + " / " + maxBench + " Units";
        }

        if (passiveText != null)
        {
            passiveText.text = fusionMessage != "" ? fusionMessage : "Total Passive: +" + totalPassive + " SP";
        }

        if (fuseButtonText != null)
        {
            fuseButtonText.text = isSelectingFusion ? "Fuse (" + fusionSlots.Count + "/3)" : "Fuse";
        }

        for (int i = 0; i < 5; i++)
        {
            if (slotButtons[i] == null)
            {
                continue;
            }

            bool isOccupied = false;
            bool canAfford = false;

            if (ReserveBench.instance != null)
            {
                isOccupied = ReserveBench.instance.IsSlotOccupied(i);
                canAfford = ReserveBench.instance.CanRecruitToSlot(i);
            }

            if (slotTitleTexts[i] != null)
            {
                slotTitleTexts[i].text = isOccupied ? ReserveBench.instance.slots[i].GetTroopName() : "EMPTY BENCH SLOT";
            }

            if (isOccupied)
            {
                slotButtons[i].interactable = true;
                if (slotButtonTexts[i] != null)
                {
                    slotButtonTexts[i].text = isSelectingFusion ? (fusionSlots.Contains(i) ? "DESELECT" : "SELECT") : "DEPLOY";
                }
            }
            else
            {
                slotButtons[i].interactable = canAfford && !isSelectingFusion;
                if (slotButtonTexts[i] != null)
                {
                    slotButtonTexts[i].text = "+ HIRE (20 SP)";
                }
            }
        }
    }

    // click first bench slot
    public void ClickSlot0()
    {
        OnSlotActionButtonClicked(0);
    }

    // click second bench slot
    public void ClickSlot1()
    {
        OnSlotActionButtonClicked(1);
    }

    // click third bench slot
    public void ClickSlot2()
    {
        OnSlotActionButtonClicked(2);
    }

    // click fourth bench slot
    public void ClickSlot3()
    {
        OnSlotActionButtonClicked(3);
    }

    // click fifth bench slot
    public void ClickSlot4()
    {
        OnSlotActionButtonClicked(4);
    }

    // handle slot button click to deploy or recruit into slot
    public void OnSlotActionButtonClicked(int slotIndex)
    {
        if (ReserveBench.instance == null)
        {
            return;
        }

        fusionMessage = "";
        if (isSelectingFusion)
        {
            if (!ReserveBench.instance.IsSlotOccupied(slotIndex))
            {
                fusionMessage = "Choose an occupied slot.";
                Refresh();
                return;
            }

            BenchUnit unit = ReserveBench.instance.slots[slotIndex];
            if (fusionSlots.Contains(slotIndex))
            {
                fusionSlots.Remove(slotIndex);
            }
            else if (fusionSlots.Count >= 3)
            {
                fusionMessage = "Too many selected units";
            }
            else if (unit.level != 3)
            {
                fusionMessage = "Can only select level 3 units for fusion.";
            }
            else if (unit.isSuper)
            {
                fusionMessage = "Super units cannot fuse again.";
            }
            else if (unit.branch == Branch.None)
            {
                fusionMessage = "Choose a unit with a branch.";
            }
            else if (fusionSlots.Count > 0 && unit.branch != ReserveBench.instance.slots[fusionSlots[0]].branch)
            {
                fusionMessage = "Choose units from same branch.";
            }
            else
            {
                fusionSlots.Add(slotIndex);
            }
            Refresh();
            return;
        }

        if (ReserveBench.instance.IsSlotOccupied(slotIndex))
        {
            Close();
            if (GridSelector.instance != null)
            {
                GridSelector.instance.StartBenchDeployment(slotIndex);
            }
        }
        else
        {
            ReserveBench.instance.RecruitToSlot(slotIndex);
        }

        Refresh();
    }
}
