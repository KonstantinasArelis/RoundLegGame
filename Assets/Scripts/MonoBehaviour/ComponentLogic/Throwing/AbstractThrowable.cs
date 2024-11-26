using UnityEngine;

public abstract class AbstractThrowable : MonoBehaviour
{
  public float throwSpeed;
  public Vector3 throwDirection;
  private float gravityTime = 0;
  private readonly float gravityScale = -10f;
  private bool isFrozen = false;

  protected void Start()
  {
    // make sure it's destroyed if nothing at some point
    Destroy(gameObject, 10f);
  }

  protected void Update()
  {
    if (isFrozen) return;
    transform.RotateAround(transform.position, transform.right, 300 * Time.deltaTime);
    transform.position += (throwDirection * throwSpeed + gravityScale * gravityTime * Vector3.up) * Time.deltaTime;
    gravityTime += Time.deltaTime;
  }

  protected void Destruct()
  {
    isFrozen = true;
    gameObject.AddComponent<Dissapearer>().Dissapear(0.1f, 0.1f);
  }
}