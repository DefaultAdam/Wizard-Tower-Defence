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

    private readonly Vector2Int[] leftPath =
    {
        new Vector2Int(0, 5),
        new Vector2Int(1, 5),
        new Vector2Int(2, 5),
        new Vector2Int(3, 5),
        new Vector2Int(4, 5),
        new Vector2Int(5, 5),
        new Vector2Int(5, 4),
        new Vector2Int(5, 3),
        new Vector2Int(5, 2),
        new Vector2Int(6, 2),
        new Vector2Int(7, 2),
        new Vector2Int(8, 2),
        new Vector2Int(9, 2),
        new Vector2Int(10, 2),
        new Vector2Int(11, 2),
        new Vector2Int(12, 2),
        new Vector2Int(12, 3),
        new Vector2Int(12, 4),
        new Vector2Int(12, 5),
        new Vector2Int(12, 6),
        new Vector2Int(12, 7),
        new Vector2Int(12, 8),
        new Vector2Int(11, 8),
        new Vector2Int(10, 8),
        new Vector2Int(9, 8),
        new Vector2Int(8, 8),
        new Vector2Int(8, 9),
        new Vector2Int(8, 10),
        new Vector2Int(8, 11),
        new Vector2Int(8, 12),
        new Vector2Int(9, 12),
        new Vector2Int(10, 12),
        new Vector2Int(11, 12),
        new Vector2Int(12, 12),
        new Vector2Int(13, 12),
        new Vector2Int(14, 12),
        new Vector2Int(15, 12),
        new Vector2Int(16, 12),
        new Vector2Int(16, 11),
        new Vector2Int(16, 10),
        new Vector2Int(16, 9),
        new Vector2Int(16, 8),
        new Vector2Int(17, 8),
        new Vector2Int(18, 8),
        new Vector2Int(19, 8),
        new Vector2Int(20, 8),
        new Vector2Int(20, 9),
        new Vector2Int(20, 10),
        new Vector2Int(20, 11),
        new Vector2Int(20, 12),
        new Vector2Int(20, 13),
        new Vector2Int(20, 14),
        new Vector2Int(21, 14),
        new Vector2Int(22, 14)
    };

    private readonly Vector2Int[] topPath =
    {
        new Vector2Int(25, 29),
        new Vector2Int(25, 28),
        new Vector2Int(25, 27),
        new Vector2Int(25, 26),
        new Vector2Int(24, 26),
        new Vector2Int(23, 26),
        new Vector2Int(22, 26),
        new Vector2Int(21, 26),
        new Vector2Int(20, 26),
        new Vector2Int(19, 26),
        new Vector2Int(18, 26),
        new Vector2Int(17, 26),
        new Vector2Int(16, 26),
        new Vector2Int(15, 26),
        new Vector2Int(15, 25),
        new Vector2Int(15, 24),
        new Vector2Int(15, 23),
        new Vector2Int(15, 22),
        new Vector2Int(15, 21),
        new Vector2Int(15, 20),
        new Vector2Int(16, 20),
        new Vector2Int(17, 20),
        new Vector2Int(18, 20),
        new Vector2Int(19, 20),
        new Vector2Int(20, 20),
        new Vector2Int(21, 20),
        new Vector2Int(22, 20),
        new Vector2Int(23, 20),
        new Vector2Int(24, 20),
        new Vector2Int(25, 20),
        new Vector2Int(26, 20),
        new Vector2Int(27, 20),
        new Vector2Int(28, 20),
        new Vector2Int(29, 20),
        new Vector2Int(30, 20),
        new Vector2Int(31, 20),
        new Vector2Int(32, 20),
        new Vector2Int(32, 21),
        new Vector2Int(32, 22),
        new Vector2Int(32, 23),
        new Vector2Int(32, 24),
        new Vector2Int(33, 24),
        new Vector2Int(34, 24),
        new Vector2Int(35, 24),
        new Vector2Int(36, 24),
        new Vector2Int(36, 23),
        new Vector2Int(36, 22),
        new Vector2Int(36, 21),
        new Vector2Int(36, 20),
        new Vector2Int(36, 19),
        new Vector2Int(36, 18),
        new Vector2Int(35, 18),
        new Vector2Int(34, 18),
        new Vector2Int(33, 18),
        new Vector2Int(32, 18),
        new Vector2Int(31, 18),
        new Vector2Int(30, 18),
        new Vector2Int(29, 18),
        new Vector2Int(28, 18),
        new Vector2Int(27, 18),
        new Vector2Int(26, 18),
        new Vector2Int(25, 18),
        new Vector2Int(25, 17)
    };

    private readonly Vector2Int[] rightPath =
    {
        new Vector2Int(49, 10),
        new Vector2Int(48, 10),
        new Vector2Int(47, 10),
        new Vector2Int(46, 10),
        new Vector2Int(45, 10),
        new Vector2Int(44, 10),
        new Vector2Int(44, 9),
        new Vector2Int(44, 8),
        new Vector2Int(44, 7),
        new Vector2Int(44, 6),
        new Vector2Int(44, 5),
        new Vector2Int(44, 4),
        new Vector2Int(43, 4),
        new Vector2Int(42, 4),
        new Vector2Int(41, 4),
        new Vector2Int(40, 4),
        new Vector2Int(39, 4),
        new Vector2Int(38, 4),
        new Vector2Int(37, 4),
        new Vector2Int(36, 4),
        new Vector2Int(35, 4),
        new Vector2Int(35, 5),
        new Vector2Int(35, 6),
        new Vector2Int(35, 7),
        new Vector2Int(35, 8),
        new Vector2Int(35, 9),
        new Vector2Int(35, 10),
        new Vector2Int(36, 10),
        new Vector2Int(37, 10),
        new Vector2Int(38, 10),
        new Vector2Int(39, 10),
        new Vector2Int(40, 10),
        new Vector2Int(40, 11),
        new Vector2Int(40, 12),
        new Vector2Int(40, 13),
        new Vector2Int(40, 14),
        new Vector2Int(39, 14),
        new Vector2Int(38, 14),
        new Vector2Int(37, 14),
        new Vector2Int(36, 14),
        new Vector2Int(35, 14),
        new Vector2Int(34, 14),
        new Vector2Int(33, 14),
        new Vector2Int(32, 14),
        new Vector2Int(31, 14),
        new Vector2Int(30, 14),
        new Vector2Int(29, 14),
        new Vector2Int(28, 14),
        new Vector2Int(27, 14)
    };

    public Vector2Int fortressMin = new Vector2Int(8, 4);
    public Vector2Int fortressMax = new Vector2Int(9, 5);

    private void Awake()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
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
                cells[x, y] = CellType.Fortress;
            }
        }

        markPath(leftPath);
        markPath(topPath);
        markPath(rightPath);
    }

    private void markPath(Vector2Int[] path)
    {
        foreach (Vector2Int position in path)
        {
            cells[position.x, position.y] = CellType.Path;
        }
    }

    public Vector3 GetCellCenter(Vector2Int position)
    {
        return transform.position + new Vector3(
            (position.x + 0.5f) * cellSize,
            0f,
            (position.y + 0.5f) * cellSize
        );
    }

    public CellType GetCellType(Vector2Int position)
    {
        if (!IsInsideGrid(position))
            return CellType.Used;

        return cells[position.x, position.y];
    }

    public void SetCellType(Vector2Int position, CellType cellType)
    {
        if (IsInsideGrid(position))
        {
            cells[position.x, position.y] = cellType;
        }
    }

    public bool IsInsideGrid(Vector2Int position)
    {
        return position.x >= 0 && position.x < width
            && position.y >= 0 && position.y < height;
    }

    private bool ContainsCell(Vector2Int[] path, Vector2Int position)
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

    private void OnDrawGizmos()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int position = new Vector2Int(x, y);

                if (IsFortressCell(position))
                {
                    Gizmos.color = Color.gray;
                }
                else
                {
                    Color pathColour;

                    if (GetPathColor(position, out pathColour))
                    {
                        Gizmos.color = pathColour;
                    }

                    else
                    {
                        Gizmos.color =
                            new Color(0.30f, 0.70f, 0.35f);
                    }
                }

                Gizmos.DrawCube(
                    GetCellCenter(position),
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
