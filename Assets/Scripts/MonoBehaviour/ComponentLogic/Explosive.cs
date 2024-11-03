using UnityEngine;

public class Explosive : MonoBehaviour
{
  public float impactRadius;
  public int damage;
  [SerializeField] private GameObject explosionFX;

  public void Explode()
  {
    // sphere raycast
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, impactRadius, LayerMask.GetMask("Enemy"));
    foreach (var hitCollider in hitColliders)
    {
      if (hitCollider.GetComponent<ZombieController>() != null)
      {
        hitCollider.GetComponent<ZombieController>().TakeDamage(damage);
      }
    }
    GameObject explosion = Instantiate(explosionFX, transform.position, transform.rotation);
    explosion.GetComponent<Dissapearer>().Dissapear();
    Destroy(gameObject);
  }
}