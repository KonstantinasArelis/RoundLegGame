using System.Collections;
using UnityEngine;

abstract public class AbstractThrower : MonoBehaviour
{
  public Transform spawnTransform;
  public float throwSpeed = 10f;
  public float throwsPerSecond = 1f;
  private GameObject prefab;


  private void Start()
  {
    // first child is the prefab to be thrown
    prefab = transform.GetChild(0).gameObject;
    prefab.SetActive(false);
    StartCoroutine(ThrowCoroutine());
  }

  protected void Throw()
  {
    GameObject thrownObject = Instantiate(prefab, spawnTransform.position, prefab.transform.rotation);
    var throwable = thrownObject.GetComponent<AbstractThrowable>();
    throwable.throwSpeed = throwSpeed;
    ThrowConfig(throwable, thrownObject);
    thrownObject.SetActive(true);
  }

  abstract protected void ThrowConfig(AbstractThrowable throwable, GameObject thrownObject);


  private IEnumerator ThrowCoroutine()
  {
    for ( ; ; )
    {
      Throw();
      yield return new WaitForSeconds(1/throwsPerSecond);
    }
  }


}