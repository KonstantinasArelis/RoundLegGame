using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
  public void Destroy(float delay)
  {
    Destroy(gameObject, delay);
  }
}