using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Scriptable Objects/Upgrades")]
public class UpgradeData : ScriptableObject
{
  public UpgradeData(UpgradeTypeEnum type)
  {
    this.type = type;
  }

  public new string name;

  public GameObject prefab;

  public Texture2D icon;

  public UpgradeTypeEnum type;

  public UpgradeData nextUpgrade;
}
