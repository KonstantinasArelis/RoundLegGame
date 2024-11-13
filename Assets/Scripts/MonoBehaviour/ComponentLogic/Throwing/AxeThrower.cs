using UnityEngine;

public class AxeThrower : AbstractThrower
{
  protected override void ThrowConfig(AbstractThrowable throwable, GameObject thrownObject)
  {
    throwable.throwDirection = (spawnTransform.forward / 2f + spawnTransform.up * 2f).normalized;
    thrownObject.transform.LookAt(spawnTransform.position + spawnTransform.forward);
  }
}
