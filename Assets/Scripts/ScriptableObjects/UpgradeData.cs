using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Scriptable Objects/Upgrades")]
public class UpgradeData : ScriptableObject
{
  public new string name;

  public GameObject prefab;

  public UpgradeTypeEnum type;

  public UpgradeTypeEnum previousUpgradeType;

  public UpgradeTierEnum tier;
}
