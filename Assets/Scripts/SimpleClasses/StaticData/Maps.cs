using System.Collections.Generic;
using System.Linq;

public static class Maps
{
  public static Dictionary<UpgradeTypeEnum, string> gunToTag = new ()
  {
    {UpgradeTypeEnum.Pistol, "Pistol"},
    {UpgradeTypeEnum.Uzi, "Uzi"},
    {UpgradeTypeEnum.Shotgun, "Shotgun"},
    {UpgradeTypeEnum.Railgun, "Railgun"},
    {UpgradeTypeEnum.DoubleUzi, "DoubleUzi"}
  };
  
  public static int[] xpNeededPerLevel = Enumerable.Range(1, 100).Select(level => XpNeededPerLevelFormula(level)).ToArray();

  private static int XpNeededPerLevelFormula(int level) => level * 10;
}