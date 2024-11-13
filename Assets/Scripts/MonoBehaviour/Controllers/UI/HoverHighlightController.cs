using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HoverHighlightController : AbstractHoverController
{
  private Color startColor;
  private RawImage rawImage;

  protected override void Start()
  {
    base.Start();
    rawImage = transform.GetComponent<RawImage>();
    startColor = rawImage.color;
  }
    protected override void PointerEnter()
    {
      rawImage.DOColor(new Color(1f, 1f, 0f, 0.6f), 0.2f);
    }

    protected override void PointerExit()
    {
      transform.GetComponent<RawImage>().color = startColor;
    }
}