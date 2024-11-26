using UnityEngine;

public class Flame : MonoBehaviour
{

  public void Activate(float activeSeconds=5f)
  {
    // place on the ground
    Transform groundTransfrom = GameObject.FindWithTag("Ground").transform;
    float groundY = groundTransfrom.GetComponent<Collider>().bounds.max.y;
    transform.position = new Vector3(transform.position.x, groundY + 0.01f, transform.position.z);
    Destroy(gameObject, activeSeconds);
  }

  void OnTriggerStay(Collider other)
  {
    if (other.gameObject.CompareTag("Enemy"))
    {
      other.gameObject.AddComponent<Burner>().Burn(5f, 1/5f);
    }
  }
}