using DG.Tweening;

public class HoverExpandController : AbstractHoverController
{
  private float initialScale;

  override protected void Start()
  {
    base.Start();
    initialScale = transform.localScale.x;
  }
  override protected void PointerEnter()
  {
    // setupdate - independent time (no time scale)
    transform.DOScale(initialScale * 1.2f, 0.2f).SetUpdate(true);
  }

  override protected void PointerExit()
  {
    // setupdate - independent time (no time scale)
    transform.DOScale(initialScale, 0.2f).SetUpdate(true);
  }
}