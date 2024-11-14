using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class PistolController : MonoBehaviour, IGunStatUpgradeable
{
    private FireLine fireLine;
    
    private Vector3 initalForward;
	[SerializeField] public float muzzleFlashDuration = 0.1f;
    [SerializeField] public float startingShotCooldownSeconds = 0.3f;
    [SerializeField] public float startingPenetration = 3f;
    [SerializeField] public float startingKnockbackForce = 3f;
    [SerializeField] public float startingBaseDamage = 1f;

    public float shotCooldownSeconds {get; set;}
    public float penetration {get; set;}
    public float knockbackForce {get; set;}
    public float baseDamage {get; set;}

    public int shotCooldownSecondsUpgradeCount {get; set;}
    public int penetrationUpgradeCount {get; set;}
    public int knockbackForceUpgradeCount {get; set;}
    public int baseDamageUpgradeCount {get; set;}

    private float lastShotTime = 0.0f;
    public VisualEffect muzzleFlash;
	public Light muzzlePointFlashLight;
    public Light muzzleDirectionalFlashLight;

    void Start()
    {
        this.shotCooldownSeconds = startingShotCooldownSeconds;
        this.penetration = startingPenetration;
        this.knockbackForce = startingKnockbackForce;
        this.baseDamage = startingBaseDamage;
        
        this.shotCooldownSecondsUpgradeCount = 0;
        this.penetrationUpgradeCount = 0;
        this.knockbackForceUpgradeCount = 0;
        this.baseDamageUpgradeCount = 0;

        fireLine = GetComponentInChildren<FireLine>();

        muzzlePointFlashLight.enabled = false;
        muzzleDirectionalFlashLight.enabled = false;
    	initalForward = transform.forward;
    }

    public void Fire()
    {
        float lastShotDifference = Time.time - lastShotTime;
		bool gunCooledDown = lastShotDifference >= shotCooldownSeconds;
		if (!gunCooledDown)
		{
			return;
		}
		
		muzzlePointFlashLight.enabled = true;
        muzzleDirectionalFlashLight.enabled = true;
		Invoke(nameof(DisableMuzzleFlashLight), muzzleFlashDuration); 
		muzzleFlash.Play();
		lastShotTime = Time.time;

        fireLine.Fire(penetration, knockbackForce, baseDamage);
    }

    void DisableMuzzleFlashLight()
    {
        muzzlePointFlashLight.enabled = false;
        muzzleDirectionalFlashLight.enabled = false;
    }

    public void IncreaseStat(GunStatPanelTypeEnum stat){

        switch(stat)
        {
            case GunStatPanelTypeEnum.BaseDamage:
                baseDamageUpgradeCount++;
                baseDamage = startingBaseDamage + baseDamageUpgradeCount*2;
            break;
            case GunStatPanelTypeEnum.ShotCooldownSeconds:
                shotCooldownSecondsUpgradeCount++;
                baseDamage = startingShotCooldownSeconds + shotCooldownSecondsUpgradeCount*2;
            break;
            case GunStatPanelTypeEnum.Penetration:
                penetrationUpgradeCount++;
                baseDamage = startingPenetration + penetrationUpgradeCount;
            break;
            case GunStatPanelTypeEnum.Knockback:
                knockbackForceUpgradeCount++;
                baseDamage = startingKnockbackForce + knockbackForceUpgradeCount*2;
            break;
        }
        Debug.Log("upgraded: " + stat);
    }
}
