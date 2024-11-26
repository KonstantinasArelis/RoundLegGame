using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/Buildings")]
public class BuildingData : ScriptableObject
{
  public GameObject prefab;
  public Texture2D icon;
  public new string name;
  public int cost;

  // Ensures cost is always negative
  private void OnValidate()
  {
    if (cost > 0)
    {
        cost = -Mathf.Abs(cost);
    }
  }
}
