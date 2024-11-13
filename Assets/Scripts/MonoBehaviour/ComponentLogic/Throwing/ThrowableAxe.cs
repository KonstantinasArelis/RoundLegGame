using UnityEngine;

public class ThrowableAxe : AbstractThrowable
{
  public float damage = 1f;

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Enemy"))
    {
      if (other.TryGetComponent<IDamagable>(out var damagable))
      {
        damagable.TakeDamage(damage);
        Destruct();
        return;
      }
    }
    if (other.CompareTag("Ground"))
    {
      Destruct();
      return;
    }
  }
}