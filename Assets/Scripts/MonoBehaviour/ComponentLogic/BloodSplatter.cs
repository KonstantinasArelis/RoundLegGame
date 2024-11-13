using DG.Tweening;
using UnityEngine;


[RequireComponent(typeof(Dissapearer))]
public class BloodSplatter : MonoBehaviour
{
  public float splatterScale = 1f;

  void Start()
  {
    // place on the ground
    Transform groundTransfrom = GameObject.Find("Plane").transform;
    float groundY = groundTransfrom.GetComponent<Collider>().bounds.max.y;
    transform.position = new Vector3(transform.position.x, groundY + 0.01f, transform.position.z);
    Vector3 previousScale = transform.localScale;
    transform.localScale = new Vector3(0, transform.localScale.y, 0);
    transform.DOScale(previousScale * splatterScale, 0.3f).OnComplete(
      () => GetComponent<Dissapearer>().Dissapear());

  }


}