using System.Collections;
using UnityEngine;

public class AxeThrower : MonoBehaviour
{
  public Transform spawnTransform;
  public float throwSpeed = 10f;
  public int throwFrequency = 1;
  private GameObject axeBlueprint;


  void Start()
  {
    axeBlueprint = transform.Find("Axe").gameObject;
    axeBlueprint.SetActive(false);
    StartCoroutine(ThrowCoroutine());
  }

  private void Throw()
  {
    GameObject thrownAxe = Instantiate(axeBlueprint, spawnTransform.position, axeBlueprint.transform.rotation);
    var thrownAxeScript = thrownAxe.AddComponent<ThrownAxe>();
    thrownAxeScript.throwSpeed = throwSpeed;
    thrownAxeScript.forceDirection = (spawnTransform.forward / 2f + spawnTransform.up * 2f).normalized;
    thrownAxe.transform.LookAt(spawnTransform.position + spawnTransform.forward);
    thrownAxe.SetActive(true);
  }

  private IEnumerator ThrowCoroutine()
  {
    for ( ; ; )
    {
      Throw();
      yield return new WaitForSeconds(1/throwFrequency);
    }
  }


}