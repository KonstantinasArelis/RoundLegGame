using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Scriptable Objects/Upgrades")]
public class UpgradeData : ScriptableObject
{
  public UpgradeData(UpgradeTypeEnum type)
  {
    this.type = type;
  }

  public new string name;

  public GameObject prefab;

  public UpgradeTypeEnum type;

  public UpgradeData[] nextUpgrades;

  public int nextUpgradeLevel;
 
  private void OnValidate()
  {
    // ensure nextUpgradeLevel is positive
    if (nextUpgradeLevel < 0)
    {
      nextUpgradeLevel = Mathf.Abs(nextUpgradeLevel);
    }

    // notify if leveling is wrong
    // for children
    for (int i = 0; i < nextUpgrades.Length; ++i)
    {
      if (nextUpgrades[i].nextUpgradeLevel < nextUpgradeLevel)
      {
        Debug.LogWarning($"upgrade '{name}': {i}th next upgrade '{nextUpgrades[i].name}' is lower than needed.");
      }
    }
  }
}
