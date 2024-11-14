using DG.Tweening;
using UnityEngine;


public class GroundSpill : MonoBehaviour
{
  public float spillScale = 1f;
  public float activeTime = 5f;

  void Start()
  {
    // place on the ground
    Transform groundTransfrom = GameObject.FindWithTag("Ground").transform;
    float groundY = groundTransfrom.GetComponent<Collider>().bounds.max.y;
    transform.position = new Vector3(transform.position.x, groundY + 0.01f, transform.position.z);
    Vector3 previousScale = transform.localScale;
    transform.localScale = new Vector3(0, transform.localScale.y, 0);
    transform.DOScale(previousScale * spillScale, 0.3f).OnComplete(
      () => gameObject.AddComponent<Dissapearer>().Dissapear(activeTime));
  }


}