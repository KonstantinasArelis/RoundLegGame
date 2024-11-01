using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/Buildings")]
public class BuildingData : ScriptableObject
{
  public GameObject prefab;
  public new string name;
  public int cost;

  public string GetCostString()
  {
    return "-" + cost;
  }
}
