using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class UziController : MonoBehaviour, IGunStatUpgradeable
{
    private FireLine fireLine;
    public AudioSource audioSource;
    private Vector3 initalForward;
    [SerializeField] public float muzzleFlashDuration = 0.1f;


    [SerializeField] public float startingShotCooldownSeconds = 0.08f;
    [SerializeField] public float startingPenetration = 2f;
    [SerializeField] public float startingKnockbackForce = 1f;
    [SerializeField] public float startingBaseDamage = 1f;

    public float shotCooldownSeconds {get; set;}
    public float penetration {get; set;}
    public float knockbackForce {get; set;}
    public float baseDamage {get; set;}

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
        audioSource.Play();
		lastShotTime = Time.time;

        fireLine.Fire(penetration, knockbackForce, baseDamage);
    }

    void DisableMuzzleFlashLight()
    {
        muzzlePointFlashLight.enabled = false;
        muzzleDirectionalFlashLight.enabled = false;
    }
}
