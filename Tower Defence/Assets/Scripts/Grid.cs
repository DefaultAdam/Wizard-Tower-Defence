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
        Used
    }

    private CellType[,] cells;
}
