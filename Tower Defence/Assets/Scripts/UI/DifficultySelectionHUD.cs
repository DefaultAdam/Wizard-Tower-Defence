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
    void Start()
    {
        if (easyButton != null)
        {
            easyButton.onClick.AddListener(SelectEasy);
        }

        if (mediumButton != null)
        {
            mediumButton.onClick.AddListener(SelectMedium);
        }

        if (hardButton != null)
        {
            hardButton.onClick.AddListener(SelectHard);
        }

        if (sizeButtons.Length > 0 && sizeButtons[0] != null)
        {
            sizeButtons[0].onClick.AddListener(SelectSmallSize);
        }

        if (sizeButtons.Length > 1 && sizeButtons[1] != null)
        {
            sizeButtons[1].onClick.AddListener(SelectMediumSize);
        }

        if (sizeButtons.Length > 2 && sizeButtons[2] != null)
        {
            sizeButtons[2].onClick.AddListener(SelectLargeSize);
        }

        if (startButton != null)
        {
            startButton.onClick.AddListener(ConfirmSelection);
            CreateBackButton();
        }

        UpdateDisplay();
    }

    void CreateBackButton()
    {
        Button backButton = Instantiate(startButton, startButton.transform.parent);
        backButton.name = "BackBtn";
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(BackToMenu);

        RectTransform rect = backButton.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200f, 50f);
        rect.anchoredPosition = new Vector2(0f, -300f);

        Image image = backButton.GetComponent<Image>();

        if (image != null)
        {
            image.color = new Color(0.38f, 0.37f, 0.38f, 0.95f);
        }

        Text label = backButton.GetComponentInChildren<Text>();

        if (label != null)
        {
            label.text = "BACK";
        }
    }

    void BackToMenu()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.ReturnToMenu();
        }
    }

    // choose easy difficulty
    public void SelectEasy()
    {
        selectedDifficulty = DifficultyLevel.Easy;
        selectedSizeIndex = 1;
        UpdateDisplay();
    }

    // choose medium difficulty
    public void SelectMedium()
    {
        selectedDifficulty = DifficultyLevel.Medium;
        selectedSizeIndex = 1;
        UpdateDisplay();
    }

    // choose hard difficulty
    public void SelectHard()
    {
        selectedDifficulty = DifficultyLevel.Hard;
        selectedSizeIndex = 1;
        UpdateDisplay();
    }

    // select first map size choice
    public void SelectSmallSize()
    {
        selectedSizeIndex = 0;
        UpdateDisplay();
    }

    // select second map size choice
    public void SelectMediumSize()
    {
        selectedSizeIndex = 1;
        UpdateDisplay();
    }

    // select third map size choice
    public void SelectLargeSize()
    {
        selectedSizeIndex = 2;
        UpdateDisplay();
    }

    // get preset by difficulty and index
    public MapSizePreset GetPreset(DifficultyLevel difficulty, int index)
    {
        MapSizePreset preset = new MapSizePreset();

        if (difficulty == DifficultyLevel.Easy)
        {
            if (index == 0)
            {
                preset.label = "Compact";
                preset.width = 40;
                preset.height = 24;
                preset.description = "Compact (40x24): Shorter paths, easier defence";
            }
            else if (index == 1)
            {
                preset.label = "Standard";
                preset.width = 50;
                preset.height = 30;
                preset.description = "Standard (50x30): Classic balanced layout";
            }
            else
            {
                preset.label = "Spacious";
                preset.width = 60;
                preset.height = 36;
                preset.description = "Spacious (60x36): Wide perimeter with extra building space";
            }
        }
        else if (difficulty == DifficultyLevel.Medium)
        {
            if (index == 0)
            {
                preset.label = "Small";
                preset.width = 45;
                preset.height = 26;
                preset.description = "Small (45x26): Tight borders, faster waves";
            }
            else if (index == 1)
            {
                preset.label = "Default";
                preset.width = 50;
                preset.height = 30;
                preset.description = "Default (50x30): Standard classic battlefield";
            }
            else
            {
                preset.label = "Expansive";
                preset.width = 55;
                preset.height = 34;
                preset.description = "Expansive (55x34): Longer paths across three lanes";
            }
        }
        else
        {
            if (index == 0)
            {
                preset.label = "Cramped";
                preset.width = 38;
                preset.height = 22;
                preset.description = "Cramped (38x22): Close enemy spawns near fortress";
            }
            else if (index == 1)
            {
                preset.label = "Grand";
                preset.width = 50;
                preset.height = 30;
                preset.description = "Grand (50x30): Tough waves across standard grounds";
            }
            else
            {
                preset.label = "Colossal";
                preset.width = 65;
                preset.height = 40;
                preset.description = "Colossal (65x40): Massive frontier, heavy resistance";
            }
        }

        return preset;
    }

    // get active preset
    public MapSizePreset GetActivePreset()
    {
        return GetPreset(selectedDifficulty, selectedSizeIndex);
    }

    // update button highlights and descriptive text
    public void UpdateDisplay()
    {
        Color selectedColour = new Color(0.95f, 0.75f, 0.2f, 1f);
        Color normalColour = new Color(0.2f, 0.25f, 0.32f, 0.9f);

        SetButtonColour(easyButton, selectedDifficulty == DifficultyLevel.Easy ? selectedColour : normalColour);
        SetButtonColour(mediumButton, selectedDifficulty == DifficultyLevel.Medium ? selectedColour : normalColour);
        SetButtonColour(hardButton, selectedDifficulty == DifficultyLevel.Hard ? selectedColour : normalColour);

        if (difficultyDescriptionText != null)
        {
            if (selectedDifficulty == DifficultyLevel.Easy)
            {
                difficultyDescriptionText.text = "EASY: 150 Starting SP, lower enemy health, calm wave timers.";
            }
            else if (selectedDifficulty == DifficultyLevel.Medium)
            {
                difficultyDescriptionText.text = "MEDIUM: 100 Starting SP, normal health and standard wave pacing.";
            }
            else
            {
                difficultyDescriptionText.text = "HARD: 60 Starting SP, higher enemy health, fast incoming waves.";
            }
        }

        for (int i = 0; i < 3; i++)
        {
            MapSizePreset preset = GetPreset(selectedDifficulty, i);

            if (i < sizeButtonTexts.Length && sizeButtonTexts[i] != null)
            {
                sizeButtonTexts[i].text = preset.label + "\n(" + preset.width + "x" + preset.height + ")";
            }

            if (i < sizeButtons.Length && sizeButtons[i] != null)
            {
                Color sizeColour = (i == selectedSizeIndex)
                    ? new Color(0.2f, 0.8f, 0.5f, 1f)
                    : normalColour;

                SetButtonColour(sizeButtons[i], sizeColour);
            }
        }

        MapSizePreset activePreset = GetActivePreset();

        if (mapSizeDescriptionText != null)
        {
            mapSizeDescriptionText.text = activePreset.description;
        }

        if (startButtonText != null)
        {
            startButtonText.text = "START BATTLE (" + selectedDifficulty.ToString().ToUpper() + " - " + activePreset.width + "x" + activePreset.height + ")";
        }
    }

    // helper to set button colour
    private void SetButtonColour(Button btn, Color col)
    {
        if (btn == null) return;
        Image img = btn.GetComponent<Image>();
        if (img != null)
        {
            img.color = col;
        }
    }

    // confirm difficulty and map size and start battle
    public void ConfirmSelection()
    {
        isConfigured = true;
        MapSizePreset preset = GetActivePreset();

        // resize playable grid
        Grid grid = FindObjectOfType<Grid>();
        if (grid != null)
        {
            grid.SetDimensions(preset.width, preset.height);
        }

        // clip environment tiles to fit chosen grid dimensions
        GameObject dungeonTiles = GameObject.Find("DungeonTiles");
        if (dungeonTiles != null && grid != null)
        {
            foreach (Transform child in dungeonTiles.transform)
            {
                // Only clip ground and path tiles to the grid bounds, preserve perimeter walls, towers, and props
                if (child.name.Contains("GroundTile") || child.name.Contains("PathTile"))
                {
                    Vector2Int cell = grid.convertWorldPosToCell(child.position);
                    bool inside = cell.x >= 0 && cell.x < preset.width &&
                                  cell.y >= 0 && cell.y < preset.height;
                    child.gameObject.SetActive(inside);
                }
            }
        }

        // frame main camera to view selected map boundaries
        Camera cam = Camera.main;
        if (cam != null)
        {
            float centerX = preset.width * 0.5f;
            float centerZ = preset.height * 0.5f;
            float heightRatio = (float)preset.height / 30f;
            float widthRatio = (float)preset.width / 50f;
            float scale = Mathf.Max(heightRatio, widthRatio);

            float targetCamY = 36.2f * scale;
            float targetCamZ = centerZ - (centerZ + 1.2f) * scale;
            cam.transform.position = new Vector3(centerX, targetCamY, targetCamZ);
        }

        // apply starting sp according to difficulty
        if (HUD.instance != null)
        {
            int startingEnergy = 100;
            if (selectedDifficulty == DifficultyLevel.Easy)
            {
                startingEnergy = 150;
            }
            else if (selectedDifficulty == DifficultyLevel.Hard)
            {
                startingEnergy = 60;
            }

            HUD.instance.startingSP = startingEnergy;
            HUD.instance.currentSP = startingEnergy;

            if (HUD.instance.spSlider != null)
            {
                HUD.instance.spSlider.value = startingEnergy;
            }

            if (HUD.instance.spText != null)
            {
                HUD.instance.spText.text = "SP: " + startingEnergy;
            }
        }

        // apply enemy health and wave pacing to spawner
        if (EnemySpawner.instance != null)
        {
            if (selectedDifficulty == DifficultyLevel.Easy)
            {
                EnemySpawner.instance.enemyBaseHealth = 80f;
                EnemySpawner.instance.timeBetweenWaves = 4f;
            }
            else if (selectedDifficulty == DifficultyLevel.Hard)
            {
                EnemySpawner.instance.enemyBaseHealth = 130f;
                EnemySpawner.instance.timeBetweenWaves = 2f;
            }
            else
            {
                EnemySpawner.instance.enemyBaseHealth = 100f;
                EnemySpawner.instance.timeBetweenWaves = 3f;
            }
        }

        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }
}
