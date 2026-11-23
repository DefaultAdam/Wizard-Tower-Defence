using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "Main";
    public string backgroundResource = "MenuBackground";

    void Awake()
    {
        Time.timeScale = 1f;

        EnsureEventSystem();
        Transform canvas = CreateCanvas();
        CreateBackground(canvas);
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

    void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
