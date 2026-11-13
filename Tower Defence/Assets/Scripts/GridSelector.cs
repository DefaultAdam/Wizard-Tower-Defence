using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GridSelector : MonoBehaviour
{
    public static GridSelector instance;
    public Camera sceneCamera;
    public Grid grid;
    public LineRenderer highlight;
    public float outlineWidth = 0.35f;
    public GameObject towerPrefab;
    public int placementCost = 20; // recruit placement cost
    public GameObject veteranPrefab; // veteran tower prefab
    public int veteranCost = 150; // veteran placement cost
    public int veteranUnlockSP = 350; // sp milestone needed to unlock veteran
    public int selectedTroopIndex = 0; // selected troop index 0 for recruit 1 for veteran
    public int maxTowers = 5; // current maximum allowed towers

    public int maxTowerLimit = 25; // absolute maximum tower limit
    public int placedTowers = 0;
    public bool isPlacing = false;
    public bool isSelectingFusion = false; // true after pressing the inspector Fuse button
    public bool isPlacingFusion = false; // preview the fused tower before consuming the originals
    public int deployingSlotIndex = -1; // bench slot currently being placed

    private Tower hoveredTower; // troop currently under the mouse
    private Tower selectedTower; // troop clicked for upgrades
    private List<Tower> fusionTowers = new List<Tower>(); // units chosen for battlefield fusion
    public int FusionSelectionCount => fusionTowers.Count;
    public string FusionSelectionMessage { get; private set; } = "";

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

        if (SkillTreeUI.instance != null && SkillTreeUI.instance.isOpen)
        {
            hoveredTower = null;

            if (highlight != null)
            {
                highlight.enabled = false;
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Tower target = selectedTower != null ? selectedTower : hoveredTower;
            if (target != null && target.level == 0)
            {
                BenchTower(target);
            }
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
            ClearSelection();
        }

        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);

        if (!isPlacing && !isPlacingFusion)
        {
            UpdateTroopSelection(ray);
        }

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
        if (isPlacingFusion)
        {
            center += new Vector3(grid.cellSize * 0.5f, 0f, grid.cellSize * 0.5f);
            halfSize = grid.cellSize * 0.99f;
        }

        float y = center.y + 0.28f;

        if (highlight != null)
        {
            highlight.SetPosition(0, new Vector3(center.x - halfSize, y, center.z - halfSize));
            highlight.SetPosition(1, new Vector3(center.x - halfSize, y, center.z + halfSize));
            highlight.SetPosition(2, new Vector3(center.x + halfSize, y, center.z + halfSize));
            highlight.SetPosition(3, new Vector3(center.x + halfSize, y, center.z - halfSize));
            highlight.SetPosition(4, new Vector3(center.x - halfSize, y, center.z - halfSize));

            if (isPlacingFusion)
            {
                SetHighlightColour(CanPlaceFusedTower(cell) ? Color.green : Color.red);
            }
            else if (isPlacing)
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

        if ((isPlacing || isPlacingFusion) && Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (isPlacingFusion)
            {
                TryPlaceFusedTower(cell, center);
            }
            else
            {
                TryPlaceTower(cell, center);
            }
        }
    }

    // update hovered troop and select it on click
    void UpdateTroopSelection(Ray ray)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        hoveredTower = GetTowerUnderMouse(ray);

        if (Input.GetMouseButtonDown(0))
        {
            if (isSelectingFusion)
            {
                ToggleFusionTower(hoveredTower);
                return;
            }

            selectedTower = hoveredTower;
        }
    }

    // toggle a candidate while keeping all chosen units inside one 3x3 area
    void ToggleFusionTower(Tower tower)
    {
        FusionSelectionMessage = "";
        if (tower == null)
        {
            FusionSelectionMessage = "Click a tower to select it.";
            return;
        }

        if (fusionTowers.Contains(tower))
        {
            fusionTowers.Remove(tower);
            tower.ApplyColour();
            return;
        }

        if (fusionTowers.Count >= 3)
        {
            FusionSelectionMessage = "Three towers selected. Deselect one to choose another.";
            return;
        }

        if (tower.level != 3 || tower.branch == Branch.None)
        {
            FusionSelectionMessage = "Choose a Level 3 tower with a specialisation branch.";
            return;
        }

        if (tower.isSuper)
        {
            FusionSelectionMessage = "Super Apprentices cannot fuse again.";
            return;
        }

        Vector2Int minCell = grid.convertWorldPosToCell(tower.transform.position);
        Vector2Int maxCell = minCell;
        foreach (Tower selected in fusionTowers)
        {
            if (selected.branch != tower.branch)
            {
                FusionSelectionMessage = "Choose towers from the same specialisation branch.";
                return;
            }

            Vector2Int cell = grid.convertWorldPosToCell(selected.transform.position);
            minCell = Vector2Int.Min(minCell, cell);
            maxCell = Vector2Int.Max(maxCell, cell);
        }

        if (maxCell.x - minCell.x <= 2 && maxCell.y - minCell.y <= 2)
        {
            fusionTowers.Add(tower);
            tower.ApplyColour(true);
        }
        else
        {
            FusionSelectionMessage = "All three towers must fit inside one 3x3 area.";
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

    // get selected troop or hovered troop if none is selected
    public Tower GetInspectedTower()
    {
        if (selectedTower != null)
        {
            return selectedTower;
        }

        return hoveredTower;
    }

    // get troop clicked for upgrades
    public Tower GetSelectedTower()
    {
        return selectedTower;
    }

    // clear hovered and selected troop
    public void ClearSelection()
    {
        foreach (Tower tower in fusionTowers)
        {
            if (tower != null)
            {
                tower.ApplyColour();
            }
        }

        fusionTowers.Clear();
        FusionSelectionMessage = "";
        isSelectingFusion = false;
        isPlacingFusion = false;
        hoveredTower = null;
        selectedTower = null;
    }

    // enter fusion selection from an eligible inspected Master
    public void StartFusionSelection()
    {
        if (isSelectingFusion)
        {
            if (fusionTowers.Count != 3)
            {
                FusionSelectionMessage = "Select three towers, then press Fuse again.";
                return;
            }

            if (!Tower.CanFuse(fusionTowers[0], fusionTowers[1], fusionTowers[2], out string reason))
            {
                FusionSelectionMessage = reason;
                return;
            }

            isPlacingFusion = true;
            FusionSelectionMessage = "";
            return;
        }

        Tower tower = GetInspectedTower();
        if (tower == null || tower.level != 3 || tower.isSuper || tower.branch == Branch.None)
        {
            return;
        }

        CancelPlacement();
        ClearSelection();
        selectedTower = tower;
        isSelectingFusion = true;
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

        if (HUD.instance != null && HUD.instance.currentSP <= placementCost)
        {
            return;
        }

        deployingSlotIndex = -1;
        selectedTroopIndex = 0;
        isPlacing = true;
        ClearSelection();

        if (HUD.instance != null)
        {
            HUD.instance.ShowUnitInspector("Recruit", "Level 0 Recruit", "Role: Stone Thrower\nCost: 20 SP\nDamage: 20\nRate: 0.8/s\nRange: 5");
            return;
        }
    }

    // start placing recruit from chosen bench slot
    public void StartBenchDeployment(int slotIndex)
    {
        if (placedTowers >= maxTowers)
        {
            return;
        }

        deployingSlotIndex = slotIndex;
        selectedTroopIndex = 0;
        isPlacing = true;
        ClearSelection();

        if (HUD.instance != null)
        {
            HUD.instance.ShowUnitInspector("Recruit", "Level 0 Recruit (Bench Slot " + (slotIndex + 1) + ")", "Deploying from bench slot " + (slotIndex + 1) + " (0 SP)\nDamage: 20\nRate: 0.8/s\nRange: 5");
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
            if (HUD.instance.currentSP < veteranUnlockSP || HUD.instance.currentSP <= veteranCost)
            {
                return;
            }
        }

        deployingSlotIndex = -1;
        selectedTroopIndex = 1;
        isPlacing = true;
        ClearSelection();

        if (HUD.instance != null)
        {
            HUD.instance.ShowUnitInspector("Veteran", "Level 1 Veteran", "Role: Energy Caster\nDamage: 35\nRate: 0.5/s\nRange: 7\nSplash: 2");
        }
    }

    // cancel tower placement mode and hide recruit inspector
    public void CancelPlacement()
    {
        isPlacing = false;
        deployingSlotIndex = -1;
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

    // origin is the lowest-x, lowest-y cell of the proposed 2x2 footprint
    public bool CanPlaceFusedTower(Vector2Int origin)
    {
        if (grid == null || fusionTowers.Count != 3)
        {
            return false;
        }

        string reason;
        if (!Tower.CanFuse(fusionTowers[0], fusionTowers[1], fusionTowers[2], out reason))
        {
            return false;
        }

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                Vector2Int cell = origin + new Vector2Int(x, y);
                if (!grid.IsInsideGrid(cell))
                {
                    return false;
                }

                Grid.CellType type = grid.GetCellType(cell);
                if (type == Grid.CellType.Free)
                {
                    continue;
                }

                bool isOriginalTowerCell = false;
                foreach (Tower tower in fusionTowers)
                {
                    if (grid.convertWorldPosToCell(tower.transform.position) == cell)
                    {
                        isOriginalTowerCell = true;
                    }
                }

                if (type != Grid.CellType.Used || !isOriginalTowerCell)
                {
                    return false;
                }
            }
        }

        return true;
    }

    // keep the first selected unit and consume the other two only after valid confirmation
    private void TryPlaceFusedTower(Vector2Int origin, Vector3 center)
    {
        if (!isPlacingFusion || !CanPlaceFusedTower(origin))
        {
            return;
        }

        Tower fusedTower = fusionTowers[0];
        foreach (Tower tower in fusionTowers)
        {
            grid.SetCellType(grid.convertWorldPosToCell(tower.transform.position), Grid.CellType.Free);
            if (tower != fusedTower)
            {
                tower.enabled = false;
                Destroy(tower.gameObject);
            }
        }

        fusedTower.isSuper = true;
        ClearSelection();
        fusedTower.transform.position = center;
        fusedTower.transform.localScale = Vector3.Scale(fusedTower.transform.localScale, new Vector3(2f, 1f, 2f));
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                grid.SetCellType(origin + new Vector2Int(x, y), Grid.CellType.Used);
            }
        }

        placedTowers -= 2;
        if (HUD.instance != null)
        {
            HUD.instance.UpdateTowerCount(placedTowers, maxTowers);
        }

        CancelPlacement();
        selectedTower = fusedTower;
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

        if (deployingSlotIndex >= 0)
        {
            cost = 0;
        }
        else if (selectedTroopIndex == 1)
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

        if (cost > 0 && HUD.instance.currentSP <= cost)
        {
            return;
        }

        if (cost > 0)
        {
            HUD.instance.DeductSP(cost);
            HUD.instance.ShowDeduction(cost);
        }
        else if (deployingSlotIndex >= 0 && ReserveBench.instance != null)
        {
            ReserveBench.instance.DeployFromSlot(deployingSlotIndex);
        }
        deployingSlotIndex = -1;

        Instantiate(prefabToPlace, center, Quaternion.identity);
        
        grid.SetCellType(cell, Grid.CellType.Used);
        placedTowers++;
        HUD.instance.UpdateTowerCount(placedTowers, maxTowers);

        CancelPlacement();
    }

    // bench an active recruit from the battlefield back to reserve bench
    public bool BenchTower(Tower tower)
    {
        if (tower == null || tower.level != 0)
        {
            return false;
        }

        if (grid == null)
        {
            return false;
        }

        if (ReserveBench.instance == null || !ReserveBench.instance.HasCapacity())
        {
            return false;
        }

        Vector2Int cell = grid.convertWorldPosToCell(tower.transform.position);
        grid.SetCellType(cell, Grid.CellType.Free);

        if (selectedTower == tower)
        {
            ClearSelection();
        }

        Destroy(tower.gameObject);

        placedTowers--;
        if (placedTowers < 0)
        {
            placedTowers = 0;
        }

        ReserveBench.instance.BenchFromField();

        if (HUD.instance != null)
        {
            HUD.instance.UpdateTowerCount(placedTowers, maxTowers);
            HUD.instance.HideUnitInspector();
        }

        return true;
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
