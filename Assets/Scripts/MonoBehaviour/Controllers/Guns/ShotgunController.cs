using UnityEngine;
using UnityEngine.VFX;
using System.Collections;


public class ShotgunController : MonoBehaviour, IGunStatUpgradeable
{
    private FireLine[] fireLines;

    private Vector3 initalForward;
	[SerializeField] public float muzzleFlashDuration = 0.1f;

    [SerializeField] public float startingShotCooldownSeconds = 0.8f;
    [SerializeField] public float startingPenetration = 1f;
    [SerializeField] public float startingKnockbackForce = 5f;
    [SerializeField] public float startingBaseDamage = 1f;
    public float shotCooldownSeconds {get; set;}
    public float penetration {get; set;}
    public float knockbackForce {get; set;}
    public float baseDamage {get; set;}

    private float lastShotTime = 0.0f;
    public VisualEffect muzzleFlash;
	public Light muzzlePointFlashLight;
    public Light muzzleDirectionalFlashLight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.shotCooldownSeconds = startingShotCooldownSeconds;
        this.penetration = startingPenetration;
        this.knockbackForce = startingKnockbackForce;
        this.baseDamage = startingBaseDamage;

        fireLines = GetComponentsInChildren<FireLine>();

        muzzlePointFlashLight.enabled = false;
        muzzleDirectionalFlashLight.enabled = false;
    	initalForward = transform.forward;
    }

    // Update is called once per frame
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

        foreach (FireLine fireLine in fireLines)
        {
            fireLine.Fire(penetration, knockbackForce, baseDamage);
        }
    }

    void DisableMuzzleFlashLight()
    {
        muzzlePointFlashLight.enabled = false;
        muzzleDirectionalFlashLight.enabled = false;
    }
}
