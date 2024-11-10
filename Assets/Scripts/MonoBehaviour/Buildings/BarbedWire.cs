using UnityEngine;

public class BarbedWire : MonoBehaviour
{
  [SerializeField] private float damage;
  [SerializeField] private float cooldownTime;
  [SerializeField] private int damageTimes;
  [SerializeField] private float knockbackForce = 2f;
  private Cooldown cooldown;

  void Start()
  {
    cooldown = new (cooldownTime);
  }

  void OnTriggerStay(Collider other)
  {
    if (!cooldown.IsReady()) return;
    if (other.CompareTag("Enemy"))
    {
      if (other.TryGetComponent<IDamagable>(out IDamagable damagable))
      {
        damagable.TakeDamage(damage);
        OnDamage();
      }
    }
  }

  private void OnDamage()
  {
    if (damageTimes <= 0)
    {
      Destroy(gameObject);
    }
    else
    {
      --damageTimes;
    }
  }
}