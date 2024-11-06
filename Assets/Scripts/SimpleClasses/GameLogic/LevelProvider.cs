public class LevelProvider
{
  private int level;
  private int xp;
  public int[] xpNeededPerLevel;

  public LevelProvider(int level, int xp, int[] xpNeededPerLevel)
  {
    this.level = level;
    this.xp = xp;
    this.xpNeededPerLevel = xpNeededPerLevel;
  }
  public LevelProvider(int[] xpNeededPerLevel)
  {
    level = 0;
    xp = 0;
    this.xpNeededPerLevel = xpNeededPerLevel;
  }

  public bool GainXp(int xp)
  {
      if (IsMaxLevelReached()) return false;
      this.xp += xp;
      bool didLevelUp = AdjustLevelFromXp();
      return didLevelUp;
  }

    public int XpNeededForCurrentLevel()
  {
    return xpNeededPerLevel[level];
  }

  private bool AdjustLevelFromXp()
  {
    bool didLevelUp = false;
    while (xp >= XpNeededForCurrentLevel())
    {
        xp -= XpNeededForCurrentLevel();
        ++level;
        didLevelUp = true;
    }
    return didLevelUp;
  }

  public bool IsMaxLevelReached()
  {
    return level == xpNeededPerLevel.Length - 1;
  }

  public int GetCurrentLevel()
  {
    return level + 1;
  }
}
