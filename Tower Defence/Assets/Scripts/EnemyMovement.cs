using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Grid grid;
    public int pathNumber = 0;
    private Vector2Int[] path;
    private int nextGridNumber;
    public float speed = 2f;
    public int fortressDamage = 8; // sp damage dealt to fortress on breach

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

    private void Update()
    {
        if (path == null)
        {
            return;
        }

        if (nextGridNumber >= path.Length)
        {
            ReachFortress();
            return;
        }

        Vector3 targetPosition = grid.GetCellCenter(path[nextGridNumber]);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            nextGridNumber++;
        }
    }

    // damage fortress sp and destroy enemy
    private void ReachFortress()
    {
        if (HUD.instance != null)
        {
            HUD.instance.DeductSP(fortressDamage);
        }

        Destroy(gameObject);
    }
}
