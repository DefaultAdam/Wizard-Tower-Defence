using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "Main";
    public string backgroundResource = "MenuBackground";

    private Font font;
    private GameObject mainPanel;
    private GameObject settingsPanel;

    void Awake()
    {
        Time.timeScale = 1f;
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        EnsureEventSystem();
        Transform canvas = CreateCanvas();
        CreateBackground(canvas);

        mainPanel = CreatePanel(canvas, "MainPanel");
        settingsPanel = CreatePanel(canvas, "SettingsPanel");

        BuildMainPanel();
        BuildSettingsPanel();

        ShowPanel(mainPanel);
    }

    void BuildMainPanel()
    {
        CreateText(mainPanel.transform, "Wizard Tower Defence", 96, new Vector2(0f, 260f), Color.white);
        CreateText(mainPanel.transform, "Defend your fortress", 36, new Vector2(0f, 170f), new Color(0.85f, 0.85f, 0.9f));
        CreateButton(mainPanel.transform, "Play", new Vector2(0f, 20f), PlayGame);
        CreateButton(mainPanel.transform, "Settings", new Vector2(0f, -90f), OpenSettings);
    }

    void BuildSettingsPanel()
    {
        CreateText(settingsPanel.transform, "Settings", 80, new Vector2(0f, 250f), Color.white);
        CreateText(settingsPanel.transform, "Work in progress", 48, new Vector2(0f, 40f), new Color(0.85f, 0.85f, 0.9f));
        CreateButton(settingsPanel.transform, "Back", new Vector2(0f, -200f), OpenMain);
    }

    void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    void OpenMain()
    {
        ShowPanel(mainPanel);
    }

    void OpenSettings()
    {
        ShowPanel(settingsPanel);
    }

    void ShowPanel(GameObject panel)
    {
        mainPanel.SetActive(panel == mainPanel);
        settingsPanel.SetActive(panel == settingsPanel);
    }

    void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }
    }

    Transform CreateCanvas()
    {
        GameObject canvasObject = new GameObject("MenuCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        return canvasObject.transform;
    }

    void CreateBackground(Transform parent)
    {
        GameObject backgroundObject = new GameObject("Background", typeof(RectTransform));
        backgroundObject.transform.SetParent(parent, false);

        RawImage background = backgroundObject.AddComponent<RawImage>();
        Texture2D texture = Resources.Load<Texture2D>(backgroundResource);

        if (texture != null)
        {
            background.texture = texture;
        }
        else
        {
            background.color = new Color(0.05f, 0.05f, 0.08f);
        }

        AspectRatioFitter fitter = backgroundObject.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fitter.aspectRatio = 16f / 9f;

        GameObject shadeObject = new GameObject("Shade", typeof(RectTransform));
        shadeObject.transform.SetParent(parent, false);
        Image shade = shadeObject.AddComponent<Image>();
        shade.color = new Color(0f, 0f, 0f, 0.45f);
        Stretch(shadeObject.GetComponent<RectTransform>());
    }

    GameObject CreatePanel(Transform parent, string panelName)
    {
        GameObject panel = new GameObject(panelName, typeof(RectTransform));
        panel.transform.SetParent(parent, false);
        Stretch(panel.GetComponent<RectTransform>());
        return panel;
    }

    void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    Text CreateText(Transform parent, string content, int size, Vector2 position, Color colour)
    {
        GameObject textObject = new GameObject(content, typeof(RectTransform));
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(1400f, size * 1.4f);
        rect.anchoredPosition = position;

        Text text = textObject.AddComponent<Text>();
        text.font = font;
        text.text = content;
        text.fontSize = size;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = colour;
        text.raycastTarget = false;
        return text;
    }

    void CreateButton(Transform parent, string label, Vector2 position, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(label + "Button", typeof(RectTransform));
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(420f, 80f);
        rect.anchoredPosition = position;

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.38f, 0.37f, 0.38f, 0.95f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        CreateText(buttonObject.transform, label, 40, Vector2.zero, Color.white);
    }
}
