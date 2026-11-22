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
    public float riseProgress = 0f; // 0 to 1 as it grows up out of the ground, the troop uses it for the height
    public float riseTime = 0.25f; // how long it takes to rise up

    private Renderer wallRenderer;
    private Color baseColour; // the wall's normal colour, we flash to white and back from this
    private float lastHitTime = -1f; // when an enemy last hit it, for the blinking
    private bool broken = false; // stops it crumbling twice

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

    // the troop colours the wall before adding this script, so grab that colour to flash back to
    void Start()
    {
        wallRenderer = GetComponent<Renderer>();

        if (wallRenderer != null)
        {
            baseColour = wallRenderer.material.color;
        }
    }

    void Update()
    {
        riseProgress = Mathf.Clamp01(riseProgress + Time.deltaTime / riseTime);

        if (wallRenderer == null)
        {
            return;
        }

        // blink white about 10 times a second while enemies are hitting it, normal colour otherwise
        bool beingHit = Time.time - lastHitTime < 0.1f;
        bool blinkOn = Mathf.Repeat(Time.time * 10f, 1f) < 0.5f;
        wallRenderer.material.color = beingHit && blinkOn ? Color.white : baseColour;
    }

    // enemies hit the wall with do thid
    public void TakeDamage(float damage)
    {
        lastHitTime = Time.time;
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Break();
        }
    }

    //wall's done, get rid of it
    public void Break()
    {
        // crumble into yellow shards the first time it breaks
        if (!broken)
        {
            broken = true;
            BoltImpact.Spawn(transform.position, baseColour);
        }

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
