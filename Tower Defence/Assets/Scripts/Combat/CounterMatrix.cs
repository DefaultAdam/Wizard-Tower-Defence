public static class CounterMatrix
{
    public static float weaknessMultiplier = 2f;

    public static float GetDamageMultiplier(Branch branch, EnemyType enemyType)
    {
        if (branch == Branch.Physical && enemyType == EnemyType.Grunt)
        {
            return weaknessMultiplier;
        }

        return 1f;
    }

    public static float GetThrowMultiplier(Branch branch, EnemyType enemyType)
    {
        if (branch == Branch.Mechanic && enemyType == EnemyType.Juggernaut)
        {
            return weaknessMultiplier;
        }

        return 1f;
    }

    public static float GetBubbleMultiplier(Branch branch, EnemyType enemyType)
    {
        if (branch == Branch.Magic && enemyType == EnemyType.Sprint)
        {
            return weaknessMultiplier;
        }

        return 1f;
    }
}
