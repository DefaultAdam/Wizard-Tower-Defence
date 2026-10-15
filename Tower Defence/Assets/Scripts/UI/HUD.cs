using UnityEngine;
using UnityEngine.UI;

// heads up display for game stats and fortress energy
public class HUD : MonoBehaviour
{
    public Text spText; // text display for current skill points
    public Text milestoneText; // text display for current milestone tier
    public Slider spSlider; // slider bar for fortress energy pool
    public GameObject gameOverPanel; // game over screen overlay
    public int startingSP = 100; // starting fortress energy pool
    public int maxSP = 500; // maximum energy value for slider bar
    public int currentSP; // current fortress energy pool

    // initialise hud on start
    void Start()
    {
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

        UpdateSPText();
        UpdateMilestoneText();
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
    }
}
