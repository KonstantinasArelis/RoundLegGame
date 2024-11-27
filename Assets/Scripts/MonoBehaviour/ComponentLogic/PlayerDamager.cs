using UnityEngine;

public class PlayerDamager : MonoBehaviour
{
  public int damage = 1;

  void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.CompareTag("Player"))
    {
      other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
      Destroy(gameObject);
    }
  } 

}