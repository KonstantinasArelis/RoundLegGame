using UnityEngine;

public class Caster : MonoBehaviour
{
  public GameObject projectile;
  public float castDelay;

  public float castSpeed;

  private float destroyAfter = 3f;

  public void Cast(Vector3 direction)
  {
    // try to get the world position of direction from the center of the object
    var castPostion = gameObject.GetComponent<Collider>().bounds.center + direction;
    GameObject instantiatedProjectile = Instantiate(projectile,
      castPostion,
      projectile.transform.rotation);
    instantiatedProjectile.transform.LookAt(castPostion + direction);
    Rigidbody projectileBody;
    if (!instantiatedProjectile.TryGetComponent<Rigidbody>(out projectileBody))
    {
      projectileBody = instantiatedProjectile.AddComponent<Rigidbody>();
    }
    projectileBody.useGravity = false;
    projectileBody.AddForce(instantiatedProjectile.transform.forward * castSpeed, ForceMode.Impulse);
    instantiatedProjectile.AddComponent<SelfDestroy>().Destroy(destroyAfter);
  }


}