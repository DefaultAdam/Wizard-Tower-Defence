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
        new Vector2Int(5, 1),
        new Vector2Int(5, 2),
        new Vector2Int(5, 3),
        new Vector2Int(5, 4),
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
        new Vector2Int(10, 6)
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
        new Vector2Int(13, 0),
        new Vector2Int(12, 0),
        new Vector2Int(11, 0),
        new Vector2Int(10, 0),
        new Vector2Int(10, 1),
        new Vector2Int(10, 2)
    };
}
