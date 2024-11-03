using UnityEngine;

public class RotateOnAwake : MonoBehaviour
{
  [SerializeField] private Vector3 rotation;
  void Awake()
  {
    transform.rotation = Quaternion.Euler(rotation);
    Destroy(this);
  }
}