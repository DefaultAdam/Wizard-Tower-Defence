using UnityEngine;
using UnityEngine.UI;

// handles difficulty and map size selection before battle starts
public class DifficultySelectionHUD : MonoBehaviour
{
    public static DifficultySelectionHUD instance; // static reference to difficulty hud

    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }

    [System.Serializable]
    public struct MapSizePreset
    {
        public string label; // display name for map preset
        public int width; // grid width in tiles
        public int height; // grid height in tiles
        public string description; // description of map dimensions
    }

    public GameObject selectionPanel; // root panel overlay for difficulty selection
    public Button easyButton; // button to choose easy difficulty
    public Button mediumButton; // button to choose medium difficulty
    public Button hardButton; // button to choose hard difficulty
    public Text difficultyDescriptionText; // text display for active difficulty details

    public Button[] sizeButtons = new Button[3]; // buttons for three map size choices
    public Text[] sizeButtonTexts = new Text[3]; // labels on map size buttons
    public Text mapSizeDescriptionText; // text display for active map size details

    public Button startButton; // button to confirm settings and start game
    public Text startButtonText; // text label on start button

    public DifficultyLevel selectedDifficulty = DifficultyLevel.Medium; // active difficulty level
    public int selectedSizeIndex = 1; // active map size preset index
    public bool isConfigured = false; // check if player confirmed selection

    // set instance on awake
    void Awake()
    {
        instance = this;
    }

    // set up preset data and button listeners on start
        // set instance on awake
    void Awake()
    {
        instance = this;
    }
}