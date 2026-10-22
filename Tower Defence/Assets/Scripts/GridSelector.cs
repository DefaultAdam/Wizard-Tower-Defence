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
    public GameObject veteranPrefab; // veteran tower prefab
    public int veteranCost = 50; // veteran placement cost
    public int veteranUnlockSP = 250; // sp milestone needed to unlock veteran
    public int selectedTroopIndex = 0; // selected troop index 0 for recruit 1 for veteran
    public int maxTowers = 5; // current maximum allowed towers

    public int maxTowerLimit = 25; // absolute maximum tower limit
    public int placedTowers = 0;
    public bool isPlacing = false;

    private Tower hoveredTower; // troop currently under the mouse
    private Tower selectedTower; // troop clicked for upgrades

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

        UpdateAllowedTowers(1);
    }

    // update allowed towers based on wave number
    public void UpdateAllowedTowers(int wave)
    {
        int waveNumber = wave;

        if (waveNumber < 1)
        {
            waveNumber = 1;
        }

        maxTowers = waveNumber + 4;

        if (maxTowers > maxTowerLimit)
        {
            maxTowers = maxTowerLimit;
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

    // find closest troop hit by the mouse ray
    Tower GetTowerUnderMouse(Ray ray)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000f);
        Tower closestTower = null;
        float closestDistance = float.MaxValue;

        foreach (RaycastHit hit in hits)
        {
            Tower tower = hit.collider.GetComponentInParent<Tower>();

            if (tower != null && hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                closestTower = tower;
            }
        }

        return closestTower;
    }

    // start tower placement mode and display recruit inspector
    public void StartPlacement()
    {
        StartRecruitPlacement();
    }

    // start recruit placement mode and display recruit inspector
    public void StartRecruitPlacement()
    {
        if (placedTowers >= maxTowers)
        {
            return;
        }

        if (HUD.instance != null && HUD.instance.currentSP < placementCost)
        {
            return;
        }

        selectedTroopIndex = 0;
        isPlacing = true;

        if (HUD.instance != null)
        {
            HUD.instance.ShowUnitInspector("Recruit", "Level 0 Recruit", "Role: Stone Thrower\nDamage: 5\nRange: 6");
        }
    }

    // start veteran placement mode and display veteran inspector
    public void StartVeteranPlacement()
    {
        if (placedTowers >= maxTowers)
        {
            return;
        }

        if (HUD.instance != null)
        {
            if (HUD.instance.currentSP < veteranUnlockSP || HUD.instance.currentSP < veteranCost)
            {
                return;
            }
        }

        selectedTroopIndex = 1;
        isPlacing = true;

        if (HUD.instance != null)
        {
            HUD.instance.ShowUnitInspector("Veteran", "Level 2 Veteran", "Role: Energy Caster\nDamage: 35\nRange: 7");
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
        if (isPlacing && selectedTroopIndex == 0)
        {
            CancelPlacement();
        }
        else
        {
            StartRecruitPlacement();
        }
    }

    // toggle veteran placement mode
    public void ToggleVeteranPlacement()
    {
        if (isPlacing && selectedTroopIndex == 1)
        {
            CancelPlacement();
        }
        else
        {
            StartVeteranPlacement();
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
            CancelPlacement();
            return;
        }

        int cost = placementCost;
        GameObject prefabToPlace = towerPrefab;

        if (selectedTroopIndex == 1)
        {
            cost = veteranCost;
            prefabToPlace = veteranPrefab;
        }

        if (prefabToPlace == null)
        {
            Debug.LogError("GridSelector: tower prefab is null");
            return;
        }

        if (HUD.instance == null)
        {
            Debug.LogError("GridSelector: HUD is null");
            return;
        }

        if (HUD.instance.currentSP < cost)
        {
            return;
        }

        HUD.instance.DeductSP(cost);
        HUD.instance.ShowDeduction(cost);

        Instantiate(prefabToPlace, center, Quaternion.identity);
        
        grid.SetCellType(cell, Grid.CellType.Used);
        placedTowers++;
        HUD.instance.UpdateTowerCount(placedTowers, maxTowers);

        CancelPlacement();
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
