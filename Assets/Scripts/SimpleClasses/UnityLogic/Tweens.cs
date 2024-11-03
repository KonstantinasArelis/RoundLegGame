using UnityEngine;
using DG.Tweening;

public static class Tweens
{
  public static void Pop(Transform transform, float targetScale, float duration)
  {
    // scale up and down
    // watch out if current scale is not 1
    transform.DOScale(targetScale, duration/2)
      .OnComplete(() => transform.DOScale(1f, duration/2));
  }
}