using System;
using System.Collections;
using UnityEngine;

public class Burner : MonoBehaviour
{
  public float burnSeconds = 5f;
  public float damagePerSecond = 0.2f;


  public void Burn(float burnSeconds, float damagePerSecond)
  {
    if (gameObject.GetComponents<Burner>().Length > 1) return;
    GameObject burnEffect = (GameObject) Resources.Load("Prefabs/FX/Burn");
    Transform burnPositionTransfrom = transform.Find("BurnPosition");
    Instantiate(burnEffect, burnPositionTransfrom.position, burnEffect.transform.rotation, burnPositionTransfrom);
    Array.ForEach(gameObject.GetComponentsInChildren<Renderer>(), renderer => renderer.material.color = Color.red);
    this.burnSeconds = burnSeconds;
    this.damagePerSecond = damagePerSecond;
    StartCoroutine(BurnCoroutine());
  }

  private IEnumerator BurnCoroutine()
  {
    float secondsBurned = 0;
    for ( ; ; )
    {
      GiveDamage();
      yield return new WaitForSeconds(1f);
      ++secondsBurned;
      if (secondsBurned >= burnSeconds) Destroy(this);
    }
  }

  private void GiveDamage()
  {
    if (TryGetComponent<IDamagable>(out var damagable))
    {
      damagable.TakeDamage(damagePerSecond);
    }
  }
}