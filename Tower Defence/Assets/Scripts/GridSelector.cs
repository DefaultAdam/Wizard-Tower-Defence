using UnityEngine;

public class GridSelector : MonoBehaviour
{
    public Camera sceneCamera;
    public Grid grid;
    public LineRenderer highlight;
    public GameObject towerPrefab;
    public int placementCost = 10;
    public int maxTowers = 7;
    public int placedTowers = 0;

    private void Start()
    {
        if (sceneCamera == null)
        {
            sceneCamera = Camera.main;
        }

        if (grid == null)
        {
            grid = FindObjectOfType<Grid>();
        }

        if (highlight != null)
        {
            highlight.positionCount = 5;
            highlight.useWorldSpace = true;
            highlight.startWidth = 0.05f;
            highlight.endWidth = 0.05f;
            highlight.loop = false;
            highlight.enabled = false;
        }
    }

    private void Update()
    {
        if (grid == null || sceneCamera == null)
        {
            return;
        }

        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);

        Plane gridPlane = new Plane(
            Vector3.up,
            new Vector3(0f, grid.transform.position.y, 0f)
        );

        if (!gridPlane.Raycast(ray, out float distance))
        {
            if (highlight != null)
            {
                highlight.enabled = false;
            }
            return;
        }

        Vector3 mouseWorldPosition = ray.GetPoint(distance);
        Vector2Int cell = grid.convertWorldPosToCell(mouseWorldPosition);

        if (!grid.IsInsideGrid(cell))
        {
            if (highlight != null)
            {
                highlight.enabled = false;
            }
            return;
        }

        Vector3 center = grid.GetCellCenter(cell);
        float halfSize = grid.cellSize * 0.48f;
        float y = center.y + 0.05f;

        if (highlight != null)
        {
            highlight.SetPosition(0, new Vector3(center.x - halfSize, y, center.z - halfSize));
            highlight.SetPosition(1, new Vector3(center.x - halfSize, y, center.z + halfSize));
            highlight.SetPosition(2, new Vector3(center.x + halfSize, y, center.z + halfSize));
            highlight.SetPosition(3, new Vector3(center.x + halfSize, y, center.z - halfSize));
            highlight.SetPosition(4, new Vector3(center.x - halfSize, y, center.z - halfSize));
            highlight.enabled = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceTower(cell, center);
        }
    }

    private void TryPlaceTower(Vector2Int cell, Vector3 center)
    {
        if (grid.GetCellType(cell) != Grid.CellType.Free)
        {
            return;
        }

        if (placedTowers >= maxTowers)
        {
            return;
        }

        if (towerPrefab == null)
        {
            Debug.LogError("GridSelector: towerPrefab is null");
            return;
        }

        if (HUD.instance == null)
        {
            Debug.LogError("GridSelector: HUD is null");
            return;
        }

        if (HUD.instance.currentSP < placementCost)
        {
            return;
        }

        HUD.instance.DeductSP(placementCost);

        Instantiate(towerPrefab, center, Quaternion.identity);
        
        grid.SetCellType(cell, Grid.CellType.Used);
        placedTowers++;
    }
}
