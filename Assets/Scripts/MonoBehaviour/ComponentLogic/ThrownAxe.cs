using UnityEngine;

public class ThrownAxe : MonoBehaviour
{
  public float damage = 1f;
  public float knockbackForce = 10f;
  public float throwSpeed = 10f;
  public Vector3 forceDirection;

  private float gravityTime = 0;
  private float gravityScale = -10f;
  private bool isFrozen = false;

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

  private void Destruct()
  {
    isFrozen = true;
    gameObject.AddComponent<Dissapearer>().Dissapear(0.1f, 0.1f);
  }

  void Update()
  {
    if (isFrozen) return;
    transform.RotateAround(transform.position, transform.right, 300 * Time.deltaTime);
    transform.position += (forceDirection * throwSpeed + gravityScale * gravityTime * Vector3.up) * Time.deltaTime;
    gravityTime += Time.deltaTime;
  }
}