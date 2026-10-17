using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int width = 18;
    public int height = 12;
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
        new Vector2Int(0, 0),
        new Vector2Int(1, 0),
        new Vector2Int(2, 0),
        new Vector2Int(2, 1),
        new Vector2Int(2, 2),
        new Vector2Int(3, 2),
        new Vector2Int(4, 2),
        new Vector2Int(4, 1),
        new Vector2Int(4, 0),
        new Vector2Int(5, 0),
        new Vector2Int(6, 0),
        new Vector2Int(6, 1),
        new Vector2Int(6, 2),
        new Vector2Int(6, 3),
        new Vector2Int(6, 4),
        new Vector2Int(7, 4)
    };

    private readonly Vector2Int[] topPath =
    {
        new Vector2Int(13, 11),
        new Vector2Int(13, 10),
        new Vector2Int(12, 10),
        new Vector2Int(11, 10),
        new Vector2Int(11, 9),
        new Vector2Int(11, 8),
        new Vector2Int(12, 8),
        new Vector2Int(13, 8),
        new Vector2Int(13, 7),
        new Vector2Int(13, 6),
        new Vector2Int(12, 6),
        new Vector2Int(11, 6),
        new Vector2Int(10, 6),
        new Vector2Int(9, 6)
    };

    private readonly Vector2Int[] rightPath =
    {
        new Vector2Int(17, 0),
        new Vector2Int(16, 0),
        new Vector2Int(15, 0),
        new Vector2Int(15, 1),
        new Vector2Int(15, 2),
        new Vector2Int(14, 2),
        new Vector2Int(13, 2),
        new Vector2Int(13, 1),
        new Vector2Int(12, 1),
        new Vector2Int(11, 1),
        new Vector2Int(10, 1),
        new Vector2Int(10, 2),
        new Vector2Int(10, 3),
        new Vector2Int(9, 3)
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
            (position.y + 0.5f) * cellSize,
            0f
        );
    }
    public CellType GetCellType(Vector2Int position)
    {
        if (!IsInsideGrid(position))
            return CellType.Used;

        return cells[position.x, position.y];
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
                        cellSize * 0.96f,
                        0.1f
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
        int y = Mathf.FloorToInt(localPosition.y / cellSize);

        return new Vector2Int(x, y);
    }
}
