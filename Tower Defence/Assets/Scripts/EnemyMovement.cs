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
    private float pauseTimer = 0f; // how long the enemy is stuck for, 0 means it can move

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

        // dont move while paused, just count down till its free again
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
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

    // stops the enemy where it is for a bit (used by powers like the magic bubble)
    // if its already paused we keep whichever time is longer so a short hit doesnt cut a long one
    public void Pause(float duration)
    {
        if (duration > pauseTimer)
        {
            pauseTimer = duration;
        }
    }

    // true while the enemy is frozen
    public bool IsPaused()
    {
        return pauseTimer > 0f;
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
