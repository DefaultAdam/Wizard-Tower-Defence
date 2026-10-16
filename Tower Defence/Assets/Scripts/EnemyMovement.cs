using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Grid grid;
    public int pathNumber = 0;
    private Vector2Int[] path;
    private int nextGridNumber;

    private void Start()
    {
        if (grid == null)
        {
            Debug.LogError("Error: Grid is null");
            enabled = false;
            return;
        }

        path = grid.GetPath(pathNumber);

        transform.position = grid.GetCellCenter(path[0]);
        nextGridNumber = 1;
    }
}
