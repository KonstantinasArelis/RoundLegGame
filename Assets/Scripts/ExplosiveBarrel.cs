using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
  public float impactRadius = 10f;
  [SerializeField] private GameObject explosionFX;

  public void Explode()
  {
    // sphere raycast
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, impactRadius, LayerMask.GetMask("Enemy"));
    foreach (var hitCollider in hitColliders)
    {
      if (hitCollider.GetComponent<ZombieController>() != null)
      {
        hitCollider.GetComponent<ZombieController>().TakeDamage(1);
        print("hit");
      }
    }
    GameObject explosion = Instantiate(explosionFX, transform.position, transform.rotation);
    explosion.GetComponent<Dissapearer>().Dissapear();
    Destroy(gameObject);
  }
}