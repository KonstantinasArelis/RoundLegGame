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
    transform.DOScale(initialScale * 1.2f, 0.2f);
  }

  override protected void PointerExit()
  {
    transform.DOScale(initialScale, 0.2f);
  }
}