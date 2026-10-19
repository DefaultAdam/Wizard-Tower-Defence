using UnityEngine;
using UnityEngine.UI;

// heads up display for game stats and fortress energy
public class HUD : MonoBehaviour
{
    public static HUD instance; // static reference to hud

    public Text spText; // text display for current skill points
    public Text milestoneText; // text display for current milestone tier
    public Text waveText; // text display for current wave
    public Text timerText; // text display for wave countdown timer
    public Slider spSlider; // slider bar for fortress energy pool
    public GameObject gameOverPanel; // game over screen overlay
    public GameObject victoryPanel; // victory screen overlay
    public GameObject pausePanel; // pause screen overlay
    public GameObject inspectorPanel; // unit inspector panel overlay
    public Text unitNameText; // text display for selected unit name
    public Text unitLevelText; // text display for selected unit level tier
    public Text unitStatsText; // text display for selected unit combat stats
    public Text spDeductionText; // text display for energy subtraction
    public Text towerCountText; // text display for placed towers count
    public float deductionDuration = 1.5f; // display duration for deduction text
    private float deductionTimer = 0f; // countdown timer for deduction display
    public int startingSP = 100; // starting fortress energy pool
    public int maxSP = 500; // maximum energy value for slider bar
    public int currentSP; // current fortress energy pool
    public int currentWave = 1; // current wave number
    public float waveTimer = 30f; // countdown timer for wave

    // set instance and reset time scale on awake
    void Awake()
    {
        instance = this;
        Time.timeScale = 1f;
    }

    // initialise hud on start
    void Start()
    {
        Time.timeScale = 1f;
        currentSP = startingSP;

        if (spSlider != null)
        {
            spSlider.maxValue = maxSP;
            spSlider.value = currentSP;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (inspectorPanel != null)
        {
            inspectorPanel.SetActive(false);
        }

        if (spDeductionText != null)
        {
            spDeductionText.text = "";
            spDeductionText.color = Color.red;
        }

        if (towerCountText != null && GridSelector.instance != null)
        {
            UpdateTowerCount(GridSelector.instance.placedTowers, GridSelector.instance.maxTowers);
        }

        UpdateSPText();
        UpdateMilestoneText();
        UpdateWaveText();
        UpdateTimerText();
    }

    // count down deduction timer and clear text
    void Update()
    {
        if (deductionTimer > 0f)
        {
            deductionTimer -= Time.deltaTime;
            if (deductionTimer <= 0f)
            {
                if (spDeductionText != null)
                {
                    spDeductionText.text = "";
                }
            }
        }
    }

    // update sp text display
    public void UpdateSPText()
    {
        if (spText != null)
        {
            spText.text = "SP: " + currentSP;
        }
    }

    // update current milestone tier text
    public void UpdateMilestoneText()
    {
        if (milestoneText == null)
        {
            return;
        }

        if (currentSP >= 500)
        {
            milestoneText.text = "Milestone: Ultimate Skill";
        }
        else if (currentSP >= 400)
        {
            milestoneText.text = "Milestone: Level 3 Master";
        }
        else if (currentSP >= 250)
        {
            milestoneText.text = "Milestone: Level 2 Veteran";
        }
        else if (currentSP >= 150)
        {
            milestoneText.text = "Milestone: Level 1 Apprentice";
        }
        else if (currentSP >= 100)
        {
            milestoneText.text = "Milestone: Level 0 Recruit";
        }
        else
        {
            milestoneText.text = "Milestone: None (Fortress Critical)";
        }
    }

    // update wave counter text
    public void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = "Wave: " + currentWave;
        }
    }

    // update wave countdown timer text
    public void UpdateTimerText()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(waveTimer);
            timerText.text = "Next Wave: " + seconds + "s";
        }
    }

    // set wave number and refresh display
    public void SetWave(int waveNumber)
    {
        currentWave = waveNumber;
        UpdateWaveText();
    }

    // set wave timer and refresh display
    public void SetTimer(float timeRemaining)
    {
        waveTimer = timeRemaining;
        UpdateTimerText();
    }

    // update sp slider display
    public void UpdateSPSlider()
    {
        if (spSlider != null)
        {
            spSlider.value = currentSP;
        }
    }

    // check if fortress energy is depleted
    public void CheckGameOver()
    {
        if (currentSP <= 0)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            Time.timeScale = 0f;
        }
    }

    // check if victory milestone is reached
    public void CheckVictory()
    {
        if (currentSP >= maxSP)
        {
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
            }

            Time.timeScale = 0f;
        }
    }

    // modify sp and update display
    public void ModifySP(int amount)
    {
        currentSP += amount;

        if (currentSP < 0)
        {
            currentSP = 0;
        }

        UpdateSPText();
        UpdateMilestoneText();
        UpdateSPSlider();
        CheckGameOver();
        CheckVictory();
    }

    // add sp to energy pool
    public void AddSP(int amount)
    {
        ModifySP(amount);
    }

    // deduct sp from energy pool
    public void DeductSP(int amount)
    {
        ModifySP(-amount);
    }

    // show energy subtraction amount in red colour
    public void ShowDeduction(int amount)
    {
        if (spDeductionText != null)
        {
            spDeductionText.text = "-" + amount;
            deductionTimer = deductionDuration;
        }
    }

    // update tower count text display
    public void UpdateTowerCount(int current, int max)
    {
        if (towerCountText != null)
        {
            towerCountText.text = "Towers: " + current + "/" + max;
        }
    }

    // display unit details in inspector panel
    public void ShowUnitInspector(string unitName, string levelText, string statsText)
    {
        if (inspectorPanel != null)
        {
            inspectorPanel.SetActive(true);
        }

        if (unitNameText != null)
        {
            unitNameText.text = unitName;
        }

        if (unitLevelText != null)
        {
            unitLevelText.text = levelText;
        }

        if (unitStatsText != null)
        {
            unitStatsText.text = statsText;
        }
    }

    // hide unit inspector panel
    public void HideUnitInspector()
    {
        if (inspectorPanel != null)
        {
            inspectorPanel.SetActive(false);
        }
    }
}
