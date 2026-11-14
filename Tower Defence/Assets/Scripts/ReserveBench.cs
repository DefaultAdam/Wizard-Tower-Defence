using UnityEngine;

[System.Serializable]
public struct BenchUnit
{
    public bool occupied;
    public int level;
    public Branch branch;
    public bool isSuper;
}

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
    public BenchUnit[] slots = new BenchUnit[5]; // unit details for each bench slot

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
                if (slots[i].occupied)
                {
                    count++;
                }
            }
        }
        benchedUnits = count;
    }

    // check if bench slot is occupied
    public bool IsSlotOccupied(int slotIndex)
    {
        if (slots == null || slotIndex < 0 || slotIndex >= slots.Length)
        {
            return false;
        }
        return slots[slotIndex].occupied;
    }

    // check if player can afford to hire into slot
    public bool CanRecruitToSlot(int slotIndex)
    {
        if (slots == null || slotIndex < 0 || slotIndex >= slots.Length)
        {
            return false;
        }

        if (slots[slotIndex].occupied)
        {
            return false;
        }

        if (HUD.instance == null)
        {
            return false;
        }

        return HUD.instance.currentSP > recruitCost;
    }

    // hire recruit directly into bench slot
    public bool RecruitToSlot(int slotIndex)
    {
        if (!CanRecruitToSlot(slotIndex))
        {
            return false;
        }

        if (HUD.instance != null)
        {
            HUD.instance.DeductSP(recruitCost);
            HUD.instance.ShowDeduction(recruitCost);
        }

        slots[slotIndex] = new BenchUnit { occupied = true };
        UpdateBenchedCount();
        UpdateUI();
        return true;
    }

    // deploy recruit from bench slot
    public bool DeployFromSlot(int slotIndex)
    {
        if (!IsSlotOccupied(slotIndex))
        {
            return false;
        }

        slots[slotIndex] = new BenchUnit();
        UpdateBenchedCount();
        UpdateUI();
        return true;
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

    // check if player can afford to recruit to bench
    public bool CanRecruitToBench()
    {
        if (!HasCapacity())
        {
            return false;
        }

        if (HUD.instance == null)
        {
            return false;
        }

        return HUD.instance.currentSP > recruitCost;
    }

    // recruit level 0 recruit to first free bench slot
    public bool RecruitToBench()
    {
        if (!CanRecruitToBench())
        {
            return false;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].occupied)
            {
                return RecruitToSlot(i);
            }
        }

        return false;
    }

    // deploy recruit from first occupied bench slot
    public bool DeployFromBench()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].occupied)
            {
                return DeployFromSlot(i);
            }
        }

        return false;
    }

    // bench recruit from battlefield to first free slot
    public bool BenchFromField()
    {
        if (!HasCapacity())
        {
            return false;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].occupied)
            {
                slots[i] = new BenchUnit { occupied = true };
                UpdateBenchedCount();
                UpdateUI();
                return true;
            }
        }

        return false;
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
