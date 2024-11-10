using UnityEngine;
using UnityEngine.VFX;

public class ShotgunController : MonoBehaviour, IFireable, IGunStatUpgradeable
{
    private FireLine[] fireLines;

    private Vector3 initalForward;

    private AudioSource audioSource;
	[SerializeField] private float muzzleFlashDuration = 0.1f;

    [SerializeField] public float startingShotCooldownSeconds = 0.8f;
    [SerializeField] public float startingPenetration = 1f;
    [SerializeField] public float startingKnockbackForce = 5f;
    [SerializeField] public float startingBaseDamage = 1f;
    public float shotCooldownSeconds {get; set;}
    public float penetration {get; set;}
    public float knockbackForce {get; set;}
    public float baseDamage {get; set;}

    public VisualEffect muzzleFlash;
	public Light muzzlePointFlashLight;
    public Light muzzleDirectionalFlashLight;
    private Cooldown cooldown;

    void Start()
    {
        this.shotCooldownSeconds = startingShotCooldownSeconds;
        this.penetration = startingPenetration;
        this.knockbackForce = startingKnockbackForce;
        this.baseDamage = startingBaseDamage;

        fireLines = GetComponentsInChildren<FireLine>();
        audioSource = GetComponent<AudioSource>();
        muzzlePointFlashLight.enabled = false;
        muzzleDirectionalFlashLight.enabled = false;
    	initalForward = transform.forward;
        cooldown = new (shotCooldownSeconds);
    }

    public void Fire()
    {
        if (!cooldown.IsReady()) return;
		
		muzzlePointFlashLight.enabled = true;
        muzzleDirectionalFlashLight.enabled = true;
		Invoke(nameof(DisableMuzzleFlashLight), muzzleFlashDuration); 
		muzzleFlash.Play();
        audioSource.Play();

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
