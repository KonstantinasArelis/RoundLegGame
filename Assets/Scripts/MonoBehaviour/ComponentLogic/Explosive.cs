using UnityEngine;

[RequireComponent(typeof(Dissapearer))]
public class Explosive : MonoBehaviour
{
  public float impactRadius;
  public int damage;
  [SerializeField] public float knockbackForce = 10;
  [SerializeField] private GameObject explosionFX;

  public void Explode()
  {
    // sphere raycast
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, impactRadius, LayerMask.GetMask("Enemy"));
    foreach (var hitCollider in hitColliders)
    {
      if (hitCollider.TryGetComponent<IKnockable>(out IKnockable knockable))
      {
        knockable.TakeKnockback(knockbackForce, transform.position);
      }
      if (hitCollider.TryGetComponent<IDamagable>(out IDamagable damagable))
      {
        damagable.TakeDamage(damage);
      }
    }
    GameObject explosion = Instantiate(explosionFX, transform.position, transform.rotation);
    explosion.GetComponent<Dissapearer>().Dissapear();
    Destroy(gameObject);
  }
}