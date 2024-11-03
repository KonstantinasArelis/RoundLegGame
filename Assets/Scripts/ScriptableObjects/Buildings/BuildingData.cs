using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/Buildings")]
public class BuildingData : ScriptableObject
{
  public GameObject prefab;
  public new string name;
  public int cost;

  // Ensures the value is always negative
  private void OnValidate()
  {
      if (cost > 0)
      {
          cost = -Mathf.Abs(cost); // Force negative
      }
  }
}
