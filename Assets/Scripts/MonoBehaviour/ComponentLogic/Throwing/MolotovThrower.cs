using UnityEngine;

public class MolotovThrower : AbstractThrower
{
  protected override void ThrowConfig(AbstractThrowable throwable, GameObject thrownObject)
  {
    Vector3 throwDirection = (spawnTransform.forward / 2f + spawnTransform.up * 2f).normalized;
    throwDirection = GetRandomPointOnCircle(throwDirection, spawnTransform.up);
    throwable.throwDirection = throwDirection;
    thrownObject.transform.LookAt(spawnTransform.position + new Vector3(throwDirection.x, 0, throwDirection.z));
  }

  Vector3 GetRandomPointOnCircle(Vector3 direction, Vector3 axis)
  {
    // Generate a random angle in degrees
    float randomAngle = Random.Range(0f, 360f);

    // Create a quaternion for rotating around the given axis
    Quaternion rotation = Quaternion.AngleAxis(randomAngle, axis);

    // Rotate the direction vector
    Vector3 randomPoint = rotation * direction;

    return randomPoint;
  }
}
