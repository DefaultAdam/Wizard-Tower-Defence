using UnityEditor;
using UnityEngine;

public class GridSelector : MonoBehaviour
{
    public Camera camera;
    public Grid grid;
    public LineRenderer highlight;

    private void Start()
    {
        if (camera == null)
        {
            camera = Camera.main;
        }

        highlight.positionCount = 5;
        highlight.useWorldSpace = true;
        highlight.startWidth = 0.03f;
        highlight.endWidth = 0.03f;
        highlight.loop = false;
        highlight.enabled = false;
    }
}
