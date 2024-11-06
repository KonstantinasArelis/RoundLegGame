using UnityEngine;

public class FlyingOrb : MonoBehaviour
{
  [SerializeField] private int radius;
  [SerializeField] Transform orbitTarget;
  [SerializeField] private float damage;
  [SerializeField] private float speed;

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
      other.GetComponent<IDamagable>().TakeDamage(damage);
    }
  }
}