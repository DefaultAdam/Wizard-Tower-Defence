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
    public float throwCooldown = 3f; // after being thrown it cant be thrown again for this long
    private float throwCooldownTimer = 0f; // time left before it can be thrown again

    public float throwDuration = 0.5f; // how long the enemy is in the air when thrown
    public float throwHeight = 2f; // how high it goes at the top of the arc
    private bool isThrown = false; // true while its flying through the air
    private float throwTimer = 0f; // how far through the throw it is
    private Vector3 throwStart; // where the throw started from
    private Vector3 throwEnd; // the cell its going to land on

    public float floatHeight = 1f; // how high a bubbled enemy floats up
    public float floatSpeed = 3f; // how fast it rises into the air
    private bool isFloating = false; // true while its stuck in a magic bubble
    public float bubbleSize = 1.8f; // bubble size compared to the enemy, a bit bigger so it fits round it
    private GameObject bubble; // the bubble sphere, only exists while its bubbled
    public float bubbleImmunity = 2f; // after a bubble pops it cant be bubbled again for this long
    private float bubbleImmunityTimer = 0f; // time left before it can be bubbled again

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

        // throw cooldown ticks down even while paused
        if (throwCooldownTimer > 0f)
        {
            throwCooldownTimer -= Time.deltaTime;
        }

        // same thing for the bubble immunity
        if (bubbleImmunityTimer > 0f)
        {
            bubbleImmunityTimer -= Time.deltaTime;
        }

        // flying through the air from a mechanic throw, cant walk till it lands
        if (isThrown)
        {
            UpdateThrow();
            return;
        }

        // dont move while paused, just count down till its free again
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;

            if (isFloating)
            {
                FloatUp();
            }

            // bubble's done, once it starts walking again it'll drift back down to the path
            if (pauseTimer <= 0f)
            {
                isFloating = false;
                PopBubble();

                // give it a couple of seconds where magic cant freeze it again
                bubbleImmunityTimer = bubbleImmunity;
            }

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

    // magic bubble - freezes the enemy like Pause but also lifts it up into the air
    public void Bubble(float duration, Material bubbleMaterial)
    {
        // bubble only just popped, let it walk for a bit so it cant be frozen forever
        if (bubbleImmunityTimer > 0f)
        {
            return;
        }

        Pause(duration);
        isFloating = true;
        ShowBubble(bubbleMaterial);
    }

    // puts a see through sphere round the enemy, if theres already one we just keep it
    void ShowBubble(Material bubbleMaterial)
    {
        if (bubble != null || bubbleMaterial == null)
        {
            return;
        }

        bubble = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        // its just for looks so get rid of the collider, dont want towers or projectiles hitting it
        Destroy(bubble.GetComponent<Collider>());

        // child of the enemy so it moves with it and gets destroyed with it if it dies
        bubble.transform.SetParent(transform, false);
        bubble.transform.localPosition = Vector3.zero;
        bubble.transform.localScale = Vector3.one * bubbleSize;
        bubble.GetComponent<Renderer>().sharedMaterial = bubbleMaterial;
    }

    // removes the bubble when the freeze runs out
    void PopBubble()
    {
        if (bubble != null)
        {
            Destroy(bubble);
            bubble = null;
        }
    }

    // raises the enemy a bit each frame till its at float height above the path
    void FloatUp()
    {
        // use the path cell its on for the ground height, not where it is now
        // (otherwise if it gets bubbled mid throw it would float from up in the air)
        int currentCell = nextGridNumber - 1;

        if (currentCell < 0)
        {
            currentCell = 0;
        }

        float groundY = grid.GetCellCenter(path[currentCell]).y;

        Vector3 position = transform.position;
        position.y = Mathf.MoveTowards(position.y, groundY + floatHeight, floatSpeed * Time.deltaTime);
        transform.position = position;
    }

    // sends the enemy back down its path by a number of cells (for the mechanic throw)
    // it flies there in an arc, see UpdateThrow
    public void PushBack(int cells)
    {
        if (path == null || cells <= 0)
        {
            return;
        }

        // was thrown not long ago, so ignore it this time (stops enemies getting juggled forever)
        if (throwCooldownTimer > 0f)
        {
            return;
        }

        throwCooldownTimer = throwCooldown;

        // enemy is somewhere between the cell before nextGridNumber and nextGridNumber,
        // so go back from the one its just left
        int landingCell = nextGridNumber - 1 - cells;

        // cant go further back than where it spawned
        if (landingCell < 0)
        {
            landingCell = 0;
        }

        // set up the flight instead of snapping straight there
        throwStart = transform.position;
        throwEnd = grid.GetCellCenter(path[landingCell]);
        throwTimer = 0f;
        isThrown = true;

        nextGridNumber = landingCell + 1;
    }

    // moves the enemy along the arc each frame until it lands
    void UpdateThrow()
    {
        throwTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(throwTimer / throwDuration);

        // straight line from start to landing spot, then lift it up with a sine so it goes up and back down
        Vector3 position = Vector3.Lerp(throwStart, throwEnd, progress);
        position.y += Mathf.Sin(progress * Mathf.PI) * throwHeight;
        transform.position = position;

        if (progress >= 1f)
        {
            isThrown = false;
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
