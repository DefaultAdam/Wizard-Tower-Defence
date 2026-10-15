using UnityEngine;
using UnityEngine.UI;

// heads up display for game stats and fortress energy
public class HUD : MonoBehaviour
{
    public Text spText; // text display for current skill points
    public int startingSP = 100; // starting fortress energy pool
    public int currentSP; // current fortress energy pool

    // initialise hud on start
    void Start()
    {
        currentSP = startingSP;
        UpdateSPText();
    }

    // update sp text display
    public void UpdateSPText()
    {
        if (spText != null)
        {
            spText.text = "SP: " + currentSP;
        }
    }

    // modify sp and update display
    public void ModifySP(int amount)
    {
        currentSP += amount;
        UpdateSPText();
    }
}
