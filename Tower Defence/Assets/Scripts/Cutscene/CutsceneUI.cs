using UnityEngine;
using UnityEngine.UI;

// full screen overlay that shows the cutscene panels, built in code so the scene doesnt need changing
public class CutsceneUI : MonoBehaviour
{
    private GameObject panel; // the dark screen that covers everything

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
        panel.SetActive(false);
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
