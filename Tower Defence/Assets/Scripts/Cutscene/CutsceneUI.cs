using UnityEngine;
using UnityEngine.UI;

// full screen overlay that shows the cutscene panels, built in code so the scene doesnt need changing
public class CutsceneUI : MonoBehaviour
{
    private GameObject panel; // the dark screen that covers everything
    private Text speakerText; // who is talking
    private Text bodyText; // what they say

    void Awake()
    {
        GameObject canvasObject = new GameObject("Cutscene Canvas");
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // sits on top of the hud and skill tree

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasObject.AddComponent<GraphicRaycaster>();

        // dark panel covering the whole screen so the game is hidden behind it
        panel = new GameObject("Cutscene Panel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvasObject.transform, false);
        Stretch(panel, Vector2.zero, Vector2.one);
        panel.GetComponent<Image>().color = new Color(0.03f, 0.03f, 0.06f, 1f);

        speakerText = CreateText("Speaker", 44, new Vector2(0.1f, 0.6f), new Vector2(0.9f, 0.75f));
        bodyText = CreateText("Body", 32, new Vector2(0.1f, 0.25f), new Vector2(0.9f, 0.6f));
        panel.SetActive(false);
    }

    // show the panel while a cutscene is playing and keep the text on the current slide
    void Update()
    {
        CutsceneSlide slide = null;

        if (CutscenePlayer.instance != null)
        {
            slide = CutscenePlayer.instance.GetCurrentSlide();
        }

        panel.SetActive(slide != null);

        if (slide != null)
        {
            speakerText.text = slide.speaker;
            bodyText.text = slide.text;
        }
    }

    // makes a white text box inside the panel
    Text CreateText(string name, int size, Vector2 min, Vector2 max)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform));
        textObject.transform.SetParent(panel.transform, false);
        Stretch(textObject, min, max);

        Text text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = size;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        return text;
    }

    // stretches a ui object between two anchor points (0 to 1 across the screen)
    void Stretch(GameObject uiObject, Vector2 min, Vector2 max)
    {
        RectTransform rect = uiObject.GetComponent<RectTransform>();
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.sizeDelta = Vector2.zero;
    }
}
