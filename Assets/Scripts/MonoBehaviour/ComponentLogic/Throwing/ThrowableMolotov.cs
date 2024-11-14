using System;
using UnityEngine;

public class ThrowableMolotov : AbstractThrowable
{
  public float damage = 1f;

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Enemy"))
    {
      if (other.TryGetComponent<IDamagable>(out var damagable))
      {
        other.gameObject.AddComponent<Burner>().Burn(5f, 1/5f);
        Destruct();
        return;
      }
    }
    if (other.CompareTag("Ground"))
    {
      GameObject flame = (GameObject) Resources.Load("Prefabs/FX/Flame");
      Instantiate(flame, transform.position, flame.transform.rotation).GetComponent<Flame>().Activate();
      Destruct();
      return;
    }
  }
}