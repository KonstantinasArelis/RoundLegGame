using UnityEngine;

public class FlyingOrb : MonoBehaviour
{
  public Transform orbitTarget;
  public int radius;
  public float damage;
  public float speed;
  public float knockbackForce = 2f;
  void FixedUpdate()
  {
    // Orbit around target keeping radius
    var x = radius * Mathf.Cos(Time.time * speed);
    var z = radius * Mathf.Sin(Time.time * speed);
    transform.position = orbitTarget.position + new Vector3(x, 0, z);
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Enemy"))
    {
      other.GetComponent<IDamagable>()?.TakeDamage(damage);
      other.GetComponent<IKnockable>()?.TakeKnockback(knockbackForce, transform.position);
    }
  }
}