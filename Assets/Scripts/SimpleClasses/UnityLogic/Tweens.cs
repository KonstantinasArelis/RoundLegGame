using UnityEngine;
using DG.Tweening;

public static class Tweens
{
  public static void Pop(Transform transform, float targetScale=1.2f, float duration=0.2f)
  {
    // scale up and down
    // suppose the scale is uniform in x, y, z
    float previousScale = transform.localScale.x;
    transform.DOScale(targetScale, duration/2)
      .OnComplete(() => transform.DOScale(previousScale, duration/2));
  }
}