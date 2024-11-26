// using Codice.Client.Common.GameUI;
using UnityEngine;


public class Explosive : MonoBehaviour
{
  public float impactRadius;
  public int damage;
  public float knockbackForce = 10;


  public void ExplodeWithDelay(float delay=0.1f)
  {
    Invoke(nameof(Explode), delay);
  }

  public void Explode()
  {
    AffectTargets();
    var explosionFx = Resources.Load("Prefabs/FX/Explosion") as GameObject;
    Instantiate(explosionFx, transform.position, transform.rotation).AddComponent<SelfDestroy>().Destroy(2f);
    Destroy(gameObject);
  }

  private void AffectTargets()
  {
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, impactRadius, LayerMask.GetMask("Enemy", "Building"));
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
      if (hitCollider.TryGetComponent<Explosive>(out Explosive explosive))
      {
        // chain explosions on other explosive buildings
        explosive.ExplodeWithDelay();
      }
    }
  }
}