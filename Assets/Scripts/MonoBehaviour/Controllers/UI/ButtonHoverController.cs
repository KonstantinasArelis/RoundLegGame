using System.Drawing;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverController : MonoBehaviour
{
  private float initialScale;

  void Start()
  {
    initialScale = transform.localScale.x;
    EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();
    EventTrigger.Entry pointerEnterEntry = new()
    {
      eventID = EventTriggerType.PointerEnter
    };
    pointerEnterEntry.callback.AddListener((data) => PointerEnter());
    eventTrigger.triggers.Add(pointerEnterEntry);
    EventTrigger.Entry pointerExitEntry = new()
    {
      eventID = EventTriggerType.PointerExit
    };
    pointerExitEntry.callback.AddListener((data) => PointerExit());
    eventTrigger.triggers.Add(pointerExitEntry);
  }
  void PointerEnter()
  {
    transform.DOScale(initialScale * 1.2f, 0.2f);
  }

  void PointerExit()
  {
    transform.DOScale(initialScale, 0.2f);
  }
}