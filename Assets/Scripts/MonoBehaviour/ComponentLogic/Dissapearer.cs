using UnityEngine;

public class Dissapearer : MonoBehaviour
{
  public float dissapearTime = 1f;
  public void Dissapear()
  {
    Invoke(nameof(Destroy), dissapearTime);
  }

  void Destroy()
  {
    Destroy(gameObject);
  }
}