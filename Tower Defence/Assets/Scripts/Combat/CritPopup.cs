using UnityEngine;

public class CritPopup : MonoBehaviour
{
    public float lifetime = 0.6f;
    public float riseSpeed = 1.5f;
    public float height = 1f;

    private TextMesh textMesh;
    private Color startColour = new Color(1f, 0.85f, 0.1f, 1f);
    private float timer;

    public static void Show(Vector3 position)
    {
        GameObject popupObject = new GameObject("CritPopup");
        popupObject.transform.position = position;
        popupObject.AddComponent<CritPopup>();
    }

    void Awake()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        textMesh = gameObject.AddComponent<TextMesh>();
        textMesh.text = "CRIT!";
        textMesh.font = font;
        textMesh.fontSize = 48;
        textMesh.characterSize = 0.08f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = startColour;

        GetComponent<MeshRenderer>().sharedMaterial = font.material;

        transform.position += new Vector3(0f, height, 0f);
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }

        Color colour = startColour;
        colour.a = 1f - timer / lifetime;
        textMesh.color = colour;

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
