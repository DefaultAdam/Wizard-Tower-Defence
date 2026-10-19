using UnityEngine;
using UnityEngine.EventSystems;

public class GridSelector : MonoBehaviour
{
    public static GridSelector instance;
    public Camera sceneCamera;
    public Grid grid;
    public LineRenderer highlight;
    public float outlineWidth = 0.35f;
    public GameObject towerPrefab;
    public int placementCost = 10;
    public int maxTowers = 7;
    public int placedTowers = 0;
    public bool isPlacing = false;

    // initialise instance on awake
    private void Awake()
    {
        instance = this;
    }

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
            highlight.startWidth = outlineWidth;
            highlight.endWidth = outlineWidth;

            Shader unlitShader = Shader.Find("Sprites/Default");
            if (unlitShader != null)
            {
                highlight.material = new Material(unlitShader);
            }

            highlight.numCornerVertices = 4;
            SetHighlightColour(Color.white);
            highlight.loop = false;
            highlight.enabled = false;
        }

        if (HUD.instance != null)
        {
            HUD.instance.UpdateTowerCount(placedTowers, maxTowers);
        }
    }

    private void Update()
    {
        if (grid == null || sceneCamera == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
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
        float halfSize = grid.cellSize * 0.49f;
        float y = center.y + 0.28f;

        if (highlight != null)
        {
            highlight.SetPosition(0, new Vector3(center.x - halfSize, y, center.z - halfSize));
            highlight.SetPosition(1, new Vector3(center.x - halfSize, y, center.z + halfSize));
            highlight.SetPosition(2, new Vector3(center.x + halfSize, y, center.z + halfSize));
            highlight.SetPosition(3, new Vector3(center.x + halfSize, y, center.z - halfSize));
            highlight.SetPosition(4, new Vector3(center.x - halfSize, y, center.z - halfSize));

            if (isPlacing)
            {
                if (grid.GetCellType(cell) == Grid.CellType.Free)
                {
                    SetHighlightColour(Color.green);
                }
                else
                {
                    SetHighlightColour(Color.red);
                }
            }
            else
            {
                SetHighlightColour(Color.white);
            }

            highlight.enabled = true;
        }

        if (isPlacing && Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            TryPlaceTower(cell, center);
        }
    }

    // start tower placement mode and display recruit inspector
    public void StartPlacement()
    {
        if (placedTowers >= maxTowers)
        {
            return;
        }

        if (HUD.instance != null && HUD.instance.currentSP < placementCost)
        {
            return;
        }

        isPlacing = true;

        if (HUD.instance != null)
        {
            HUD.instance.ShowUnitInspector("Recruit", "Level 0 Recruit", "Role: Stone Thrower\nDamage: 5\nRange: 10");
        }
    }

    // cancel tower placement mode and hide recruit inspector
    public void CancelPlacement()
    {
        isPlacing = false;
        if (highlight != null)
        {
            highlight.enabled = false;
        }

        if (HUD.instance != null)
        {
            HUD.instance.HideUnitInspector();
        }
    }

    // toggle tower placement mode
    public void TogglePlacement()
    {
        if (isPlacing)
        {
            CancelPlacement();
        }
        else
        {
            StartPlacement();
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

    // set highlight line colour
    private void SetHighlightColour(Color color)
    {
        if (highlight == null)
        {
            return;
        }

        color.a = 1f;
        highlight.startColor = color;
        highlight.endColor = color;

        if (highlight.material != null)
        {
            highlight.material.color = color;
        }
    }
}
