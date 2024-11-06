using UnityEngine;

public class DegradableTouchDamager : MonoBehaviour
{
  [SerializeField] private float damage;
  [SerializeField] private float cooldownTime;
  [SerializeField] private int damageTimes;
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
      other.GetComponent<IDamagable>().TakeDamage(damage);
      OnDamage();
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