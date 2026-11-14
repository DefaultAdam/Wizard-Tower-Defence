using System.Collections.Generic;
using UnityEngine;

// wall made by imaginary troops, sits on one path cell and enemies have to break it to get past
public class ImaginaryWall : MonoBehaviour
{
    // every wall currently on the map, so enemies can check if theres one in front of them
    private static List<ImaginaryWall> activeWalls = new List<ImaginaryWall>();

    public Vector2Int cell; // the path cell this wall is blocking
    public float maxHealth = 100f; // how much damage it takes to break
    public float currentHealth; // health left

    // set which cell its on and how tough it is
    public void Setup(Vector2Int wallCell, float health)
    {
        cell = wallCell;
        maxHealth = health;
        currentHealth = health;
    }

    // adds its swlf to list
    void OnEnable()
    {
        activeWalls.Add(this);
    }

    // and take itself off when its gone, otherwise enemies would get stuck on a wall that doesn't exist
    void OnDisable()
    {
        activeWalls.Remove(this);
    }

    // enemies hit the wall with do thid
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Break();
        }
    }

    //wall's done, get rid of it
    public void Break()
    {
        gameObject.SetActive(false); // stop blocking immediately, before deferred destruction
        Destroy(gameObject);

    }

    // returns the wall on a cell, or null if the cell is clear
    public static ImaginaryWall GetWallAt(Vector2Int checkCell)
    {
        foreach (ImaginaryWall wall in activeWalls)
        {
            if (wall != null && wall.cell == checkCell)
            {
                return wall;
            }
        }

        return null;
    }
}
