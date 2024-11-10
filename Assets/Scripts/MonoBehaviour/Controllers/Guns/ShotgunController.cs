using UnityEngine;
using UnityEngine.VFX;

public class ShotgunController : MonoBehaviour, IFireable
{
    private FireLine[] fireLines;

    private Vector3 initalForward;

    private AudioSource audioSource;
	[SerializeField] private float muzzleFlashDuration = 0.1f;
    [SerializeField] private float shotCooldownSeconds = 0.03f;
    public VisualEffect muzzleFlash;
	public Light muzzlePointFlashLight;
    public Light muzzleDirectionalFlashLight;
    private Cooldown cooldown;

    void Start()
    {
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
            fireLine.Fire();
        }
    }

    void DisableMuzzleFlashLight()
    {
        muzzlePointFlashLight.enabled = false;
        muzzleDirectionalFlashLight.enabled = false;
    }
}
