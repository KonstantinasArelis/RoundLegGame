using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Scriptable Objects/Upgrades")]
public class UpgradeData : ScriptableObject
{
  public new string name;

  public GameObject prefab;

  public UpgradeTypeEnum type;

  public UpgradeData[] nextUpgrades;

  public int nextUpgradeLevel;

    // Ensures the nextUpgradeLevel is positive
  private void OnValidate()
  {
    if (nextUpgradeLevel < 0)
    {
      nextUpgradeLevel = Mathf.Abs(nextUpgradeLevel);
    }
  }
}
