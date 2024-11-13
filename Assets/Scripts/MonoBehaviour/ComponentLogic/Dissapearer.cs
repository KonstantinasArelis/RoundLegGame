using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Dissapearer : MonoBehaviour
{
  private float dissapearTime = 5f;
  private float fadeOutAnimationTime = 0.4f;
  private Material[] materials;

  private void Start()
  {
    var renderers = GetComponentsInChildren<Renderer>();
    foreach (Renderer renderer in renderers)
    {
      if (renderer != null)
      {
        // Clone each material to avoid changing shared materials
        materials = renderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = new Material(materials[i]);
        }
        renderer.materials = materials;
      }
    }
  }

  private IEnumerator DestroyCoroutine()
  {
    yield return new WaitForSeconds(dissapearTime);
    Array.ForEach(materials, m => Utility.SetMaterialToTransparent(m));
    FadeOut();
    yield return new WaitForSeconds(fadeOutAnimationTime);
    Destroy(gameObject);
  }

  private void FadeOut()
  {
    foreach (var mat in materials)
    {
      Color color = mat.color;
      DOTween.To(() => color.a, x => color.a = x, 0, fadeOutAnimationTime)
        .OnUpdate(() => mat.color = color);
    }
  }

  public void Dissapear(float time)
  {
    dissapearTime = time;
    Dissapear();
  }

  public void Dissapear(float time, float fadeOutAnimationTime)
  {
    dissapearTime = time;
    this.fadeOutAnimationTime = fadeOutAnimationTime;
    Dissapear();
  }

  public void Dissapear()
  {
    StartCoroutine(DestroyCoroutine());
  }
}