using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public static class Tweens
{
  public static TweenerCore<Vector3, Vector3, VectorOptions> Pop(Transform transform, float scaleFactor=1.2f, float duration=0.2f)
  {
    // scale up and down
    Vector3 previousScale = transform.localScale;
    return transform.DOScale(previousScale * scaleFactor, duration/2)
      .OnComplete(() => transform.DOScale(previousScale, duration/2));
  }
}