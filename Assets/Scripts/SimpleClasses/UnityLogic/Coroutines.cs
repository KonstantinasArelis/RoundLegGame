using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class Coroutines
{
  public static IEnumerator WithCallback(this IEnumerator coroutine, Action callback)
  {
      // Run the coroutine until it's finished
      while (coroutine.MoveNext())
      {
          yield return coroutine.Current;
      }

      // Once the coroutine is complete, invoke the callback
      callback?.Invoke();
  }

  public static IEnumerator DelayedLayoutRebuildCoroutine(GameObject container)
  {
      yield return new WaitForEndOfFrame();
      LayoutRebuilder.ForceRebuildLayoutImmediate(container.GetComponent<RectTransform>());
  }

  public static IEnumerator WaitForThumbnailCoroutine(GameObject prefab, RawImage rawImage)
  {
      Texture2D thumbnail;

      while ((thumbnail = AssetPreview.GetAssetPreview(prefab)) == null)
      {
          yield return null;  // Wait until thumbnail is generated
      }
      rawImage.texture = thumbnail;
      // provides unchanged color
      rawImage.color = new Color(1, 1, 1, 1);
  }
}