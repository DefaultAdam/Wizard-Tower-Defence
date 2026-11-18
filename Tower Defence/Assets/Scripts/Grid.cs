using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int width = 50;
    public int height = 30;
    public float cellSize = 1f;

    public enum CellType
    {
        Free,
        Path,
        Used,
        Fortress
    }

    private CellType[,] cells;

        // active path coordinate arrays
    private Vector2Int[] leftPath;
    private Vector2Int[] topPath;
    private Vector2Int[] rightPath;

    public Material pathMaterial; // material for path tiles
    public Material groundMaterial; // material for ground tiles
public Vector2Int fortressMin = new Vector2Int(23, 13);
    public Vector2Int fortressMax = new Vector2Int(26, 16);

    private void Awake()
    {
        GenerateGrid();
    }

    // resize grid and regenerate cells
    public void SetDimensions(int newWidth, int newHeight)
    {
        width = newWidth;
        height = newHeight;
        GenerateGrid();
    }

    // create grid array and mark fortress and paths
    public void GenerateGrid()
    {
        // recalculate fortress center for current grid size
        int fcx = width / 2;
        int fcy = height / 2;
        fortressMin = new Vector2Int(fcx - 2, fcy - 2);
        fortressMax = new Vector2Int(fcx + 1, fcy + 1);

        // build paths for current grid size
        BuildPathPresets();

        cells = new CellType[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y] = CellType.Free;
            }
        }

        for (int x = fortressMin.x; x <= fortressMax.x; x++)
        {
            for (int y = fortressMin.y; y <= fortressMax.y; y++)
            {
                if (IsInsideGrid(new Vector2Int(x, y)))
                {
                    cells[x, y] = CellType.Fortress;
                }
            }
        }

        if (leftPath != null) markPath(leftPath);
        if (topPath != null) markPath(topPath);
        if (rightPath != null) markPath(rightPath);
    }

    // select dedicated path layout based on grid size
    private void BuildPathPresets()
    {
        if (width <= 45)
        {
            BuildSmallPaths();
        }
        else if (width <= 50)
        {
            BuildMediumPaths();
        }
        else
        {
            BuildLargePaths();
        }
    }

    // small map layout: shorter paths and faster pressure
    private void BuildSmallPaths()
    {
        Vector2Int[] smallLeftTurns = new Vector2Int[]
        {
            new Vector2Int(0, 5), new Vector2Int(5, 5), new Vector2Int(5, 2),
            new Vector2Int(12, 2), new Vector2Int(12, 8), new Vector2Int(16, 8),
            new Vector2Int(20, 8), new Vector2Int(20, 14), new Vector2Int(22, 14)
        };

        Vector2Int[] smallTopTurns = new Vector2Int[]
        {
            new Vector2Int(25, 26), new Vector2Int(20, 26), new Vector2Int(20, 20),
            new Vector2Int(25, 20), new Vector2Int(25, 17)
        };

        Vector2Int[] smallRightTurns = new Vector2Int[]
        {
            new Vector2Int(44, 4), new Vector2Int(35, 4), new Vector2Int(35, 10),
            new Vector2Int(35, 14), new Vector2Int(27, 14)
        };

        leftPath = ConnectTurns(smallLeftTurns);
        topPath = ConnectTurns(smallTopTurns);
        rightPath = ConnectTurns(smallRightTurns);
    }

    // medium map layout: balanced classic routes
    private void BuildMediumPaths()
    {
        Vector2Int[] medLeftTurns = new Vector2Int[]
        {
            new Vector2Int(0, 5), new Vector2Int(5, 5), new Vector2Int(5, 2),
            new Vector2Int(12, 2), new Vector2Int(12, 8), new Vector2Int(8, 8),
            new Vector2Int(8, 12), new Vector2Int(16, 12), new Vector2Int(16, 8),
            new Vector2Int(20, 8), new Vector2Int(20, 14), new Vector2Int(22, 14)
        };

        Vector2Int[] medTopTurns = new Vector2Int[]
        {
            new Vector2Int(25, 29), new Vector2Int(25, 26), new Vector2Int(15, 26),
            new Vector2Int(15, 20), new Vector2Int(32, 20), new Vector2Int(32, 24),
            new Vector2Int(36, 24), new Vector2Int(36, 18), new Vector2Int(25, 18),
            new Vector2Int(25, 17)
        };

        Vector2Int[] medRightTurns = new Vector2Int[]
        {
            new Vector2Int(49, 10), new Vector2Int(44, 10), new Vector2Int(44, 4),
            new Vector2Int(35, 4), new Vector2Int(35, 10), new Vector2Int(40, 10),
            new Vector2Int(40, 14), new Vector2Int(27, 14)
        };

        leftPath = ConnectTurns(medLeftTurns);
        topPath = ConnectTurns(medTopTurns);
        rightPath = ConnectTurns(medRightTurns);
    }

    // large map layout: long sweeping marathon paths
    private void BuildLargePaths()
    {
        Vector2Int[] largeLeftTurns = new Vector2Int[]
        {
            new Vector2Int(0, 5), new Vector2Int(5, 5), new Vector2Int(5, 2),
            new Vector2Int(12, 2), new Vector2Int(12, 8), new Vector2Int(8, 8),
            new Vector2Int(8, 15), new Vector2Int(12, 15), new Vector2Int(12, 12),
            new Vector2Int(16, 12), new Vector2Int(16, 8), new Vector2Int(20, 8),
            new Vector2Int(20, 14), new Vector2Int(22, 14)
        };

        Vector2Int[] largeTopTurns = new Vector2Int[]
        {
            new Vector2Int(25, 29), new Vector2Int(25, 26), new Vector2Int(15, 26),
            new Vector2Int(15, 20), new Vector2Int(20, 20), new Vector2Int(20, 24),
            new Vector2Int(24, 24), new Vector2Int(24, 20), new Vector2Int(32, 20),
            new Vector2Int(32, 24), new Vector2Int(36, 24), new Vector2Int(36, 18),
            new Vector2Int(25, 18), new Vector2Int(25, 17)
        };

        Vector2Int[] largeRightTurns = new Vector2Int[]
        {
            new Vector2Int(49, 10), new Vector2Int(44, 10), new Vector2Int(44, 4),
            new Vector2Int(35, 4), new Vector2Int(35, 10), new Vector2Int(40, 10),
            new Vector2Int(40, 12), new Vector2Int(44, 12), new Vector2Int(44, 14),
            new Vector2Int(27, 14)
        };

        leftPath = ConnectTurns(largeLeftTurns);
        topPath = ConnectTurns(largeTopTurns);
        rightPath = ConnectTurns(largeRightTurns);
    }

    // connect turning points into contiguous cells
    private Vector2Int[] ConnectTurns(Vector2Int[] turns)
    {
        List<Vector2Int> pathCells = new List<Vector2Int>();

        for (int i = 0; i < turns.Length - 1; i++)
        {
            Vector2Int current = turns[i];
            Vector2Int target = turns[i + 1];

            while (current.x != target.x)
            {
                if (!pathCells.Contains(current)) pathCells.Add(current);
                current.x += (target.x > current.x) ? 1 : -1;
            }

            while (current.y != target.y)
            {
                if (!pathCells.Contains(current)) pathCells.Add(current);
                current.y += (target.y > current.y) ? 1 : -1;
            }
        }

        Vector2Int finalPoint = turns[turns.Length - 1];
        if (!pathCells.Contains(finalPoint)) pathCells.Add(finalPoint);

        return pathCells.ToArray();
    }

    // mark path cells inside grid bounds
    private void markPath(Vector2Int[] path)
    {
        foreach (Vector2Int position in path)
        {
            if (IsInsideGrid(position))
            {
                cells[position.x, position.y] = CellType.Path;
            }
        }
    }

    public bool IsCellFree(Vector2Int position)
    {
        if (!IsInsideGrid(position))
            return false;

        return cells[position.x, position.y] == CellType.Free;
    }

    public bool SetCellType(Vector2Int position, CellType type)
    {
        if (!IsInsideGrid(position))
            return false;

        cells[position.x, position.y] = type;
        return true;
    }

    public CellType GetCellType(Vector2Int position)
    {
        if (!IsInsideGrid(position))
            return CellType.Used;

        return cells[position.x, position.y];
    }

    public bool IsInsideGrid(Vector2Int position)
    {
        return position.x >= 0 && position.x < width &&
               position.y >= 0 && position.y < height;
    }

    bool ContainsCell(Vector2Int[] path, Vector2Int position)
    {
        foreach (Vector2Int pathPosition in path)
        {
            if (pathPosition == position)
                return true;
        }

        return false;
    }

    private bool GetPathColor(Vector2Int position, out Color color)
    {
        if (ContainsCell(leftPath, position))
        {
            color = new Color(0.85f, 0.45f, 0.20f);
            return true;
        }

        if (ContainsCell(topPath, position))
        {
            color = new Color(0.20f, 0.60f, 0.90f);
            return true;
        }

        if (ContainsCell(rightPath, position))
        {
            color = new Color(0.65f, 0.35f, 0.85f);
            return true;
        }

        color = Color.white;
        return false;
    }

    private bool IsFortressCell(Vector2Int position)
    {
        return position.x >= fortressMin.x
            && position.x <= fortressMax.x
            && position.y >= fortressMin.y
            && position.y <= fortressMax.y;
    }

    public Vector3 GetCellCenter(Vector2Int cell)
    {
        return new Vector3(
            (cell.x + 0.5f) * cellSize,
            0f,
            (cell.y + 0.5f) * cellSize
        );
    }

    private void OnDrawGizmos()
    {
        if (cells == null)
            return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                Vector3 center = GetCellCenter(pos);

                Color pathColor;
                if (GetPathColor(pos, out pathColor))
                {
                    Gizmos.color = pathColor;
                }
                else if (IsFortressCell(pos))
                {
                    Gizmos.color = new Color(0.25f, 0.85f, 0.40f);
                }
                else
                {
                    Gizmos.color = new Color(0.25f, 0.25f, 0.25f);
                }

                Gizmos.DrawWireCube(
                    center,
                    new Vector3(
                        cellSize * 0.96f,
                        0.05f,
                        cellSize * 0.96f
                    )
                );
            }
        }
    }

    public Vector2Int[] GetPath(int pathIndex)
    {
        switch (pathIndex)
        {
            case 0:
                return leftPath;

            case 1:
                return topPath;

            case 2:
                return rightPath;

            default:
                return null;
        }
    }

    public Vector2Int convertWorldPosToCell(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - transform.position;

        int x = Mathf.FloorToInt(localPosition.x / cellSize);
        int y = Mathf.FloorToInt(localPosition.z / cellSize);

        return new Vector2Int(x, y);
    }
}
