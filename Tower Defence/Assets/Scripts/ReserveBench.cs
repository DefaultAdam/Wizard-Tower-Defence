using UnityEngine;

// reserve bench manager for unplaced units and passive sp generation
public class ReserveBench : MonoBehaviour
{
    public static ReserveBench instance; // static reference to reserve bench

    public int benchedUnits = 0; // number of units waiting on bench
    public int maxBenchUnits = 5; // maximum capacity of bench
    public int recruitCost = 20; // sp fee to hire recruit
    public float passiveMultiplier = 1.5f; // passive sp multiplier
    public int totalPassiveSPEarned = 0; // total passive sp earned
    public bool allowPassiveOutsideSpawner = false; // fallback for testing outside spawner
    public bool[] slots = new bool[5]; // bench slots array

    // initialise instance on awake
    void Awake()
    {
        instance = this;
    }

    // refresh hud on start
    void Start()
    {
        UpdateBenchedCount();
        UpdateUI();
    }

    // update count of benched units from slots
    public void UpdateBenchedCount()
    {
        int count = 0;
        if (slots != null)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i])
                {
                    count++;
                }
            }
        }
        benchedUnits = count;
    }

    // check if bench has free space
    public bool HasCapacity()
    {
        UpdateBenchedCount();
        return benchedUnits < maxBenchUnits;
    }

    // check if wave is currently active
    public bool IsWaveActive()
    {
        if (EnemySpawner.instance != null)
        {
            return EnemySpawner.instance.isWaveInProgress;
        }

        return allowPassiveOutsideSpawner;
    }

    // calculate passive sp reward for one unit
    public int CalculateUnitPassiveSP(int baseKillReward)
    {
        return Mathf.RoundToInt(baseKillReward * passiveMultiplier);
    }

    // calculate total passive sp reward for all benched units
    public int CalculateTotalPassiveSP(int baseKillReward)
    {
        UpdateBenchedCount();
        if (benchedUnits <= 0)
        {
            return 0;
        }

        return CalculateUnitPassiveSP(baseKillReward) * benchedUnits;
    }

    // process kill reward during active wave and give passive sp
    public int OnEnemyKilled(int baseKillReward)
    {
        if (!IsWaveActive())
        {
            return 0;
        }

        int passiveSP = CalculateTotalPassiveSP(baseKillReward);
        if (passiveSP > 0)
        {
            totalPassiveSPEarned += passiveSP;

            if (HUD.instance != null)
            {
                HUD.instance.AddSP(passiveSP);
                HUD.instance.ShowPassiveSPBonus(passiveSP);
            }
        }

        return passiveSP;
    }

    // refresh hud displays
    public void UpdateUI()
    {
        if (HUD.instance != null)
        {
            HUD.instance.UpdateBenchCount(benchedUnits, maxBenchUnits);
            HUD.instance.UpdateBenchButtonState();
        }

        if (ReserveBenchUI.instance != null)
        {
            ReserveBenchUI.instance.Refresh();
        }
    }
}
