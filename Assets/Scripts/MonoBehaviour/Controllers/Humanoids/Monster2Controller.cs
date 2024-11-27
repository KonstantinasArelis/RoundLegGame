using System.Collections;
using UnityEngine;

public class Monster2Controller : EnemyController
{
  private Caster caster;

  void Start()
  {
    GameObject projectile = Resources.Load("Prefabs/Projectiles/ShadowBall") as GameObject;
    caster = gameObject.AddComponent<Caster>();
    caster.castDelay = 5f;
    caster.projectile = projectile;
    caster.castSpeed = 10f;
    StartCoroutine(CastCoroutine());
  }

  private IEnumerator CastCoroutine()
  {
    for ( ; ; )
    {
      caster.Cast(transform.forward);
      animator.SetTrigger("Hitting");
      yield return new WaitForSeconds(caster.castDelay);
    }
  }
}
