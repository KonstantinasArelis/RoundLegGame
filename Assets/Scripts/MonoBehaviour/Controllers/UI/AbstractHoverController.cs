using UnityEngine;
using UnityEngine.EventSystems;

public abstract class AbstractHoverController : MonoBehaviour
{

  virtual protected void Start()
  {
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
  abstract protected void PointerEnter();

  abstract protected void PointerExit();
}