using UnityEditor;
using UnityEngine;

public class GridSelector : MonoBehaviour
{
    public Camera sceneCamera;
    public Grid grid;
    public LineRenderer highlight;

    private void Start()
    {
        if (sceneCamera == null)
        {
            sceneCamera = Camera.main;
        }

        highlight.positionCount = 5;
        highlight.useWorldSpace = true;
        highlight.startWidth = 0.03f;
        highlight.endWidth = 0.03f;
        highlight.loop = false;
        highlight.enabled = false;
    }

    private void Update()
    {
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);

        Plane gridPlane = new Plane(
            Vector3.forward,
            new Vector3(0f, 0f, grid.transform.position.z)
        );

        if (!gridPlane.Raycast(ray, out float distance))
        {
            highlight.enabled = false;
            return;
        }

        Vector3 mouseWorldPosition = ray.GetPoint(distance);
        Vector2Int cell = grid.convertWorldPosToCell(mouseWorldPosition);

        if (!grid.IsInsideGrid(cell))
        {
            highlight.enabled = false;
            return;
        }

        Vector3 center = grid.GetCellCenter(cell);
        float halfSize = grid.cellSize * 0.45f;
        float z = center.z - 0.01f;

        highlight.SetPosition(0, new Vector3(
            center.x - halfSize, center.y - halfSize, z));
        highlight.SetPosition(1, new Vector3(
            center.x - halfSize, center.y + halfSize, z));
        highlight.SetPosition(2, new Vector3(
            center.x + halfSize, center.y + halfSize, z));
        highlight.SetPosition(3, new Vector3(
            center.x + halfSize, center.y - halfSize, z));
        highlight.SetPosition(4, new Vector3(
            center.x - halfSize, center.y - halfSize, z));

        highlight.enabled = true;
    }
}
