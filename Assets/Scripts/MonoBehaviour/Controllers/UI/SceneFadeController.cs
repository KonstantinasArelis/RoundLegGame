using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeController : MonoBehaviour
{
  private readonly float fadeTime = 2f;

  private RawImage fadeImage;

  void Awake()
  {
    fadeImage = GetComponentInChildren<RawImage>();
  }

  private void Fade(bool shouldFadeIn, Action callback = null)
  {
    float alpha = shouldFadeIn ? 1f : 0f;
    fadeImage.color = fadeImage.color.WithTweakedAlpha(alpha);
    // ease slow to fast
    fadeImage.DOFade(1 - alpha, fadeTime).OnComplete(() =>
    {
      callback?.Invoke();
    });
  }

  public void FadeOut(Action callback = null) => Fade(false, callback);

  public void FadeIn(Action callback = null) => Fade(true, callback);
}