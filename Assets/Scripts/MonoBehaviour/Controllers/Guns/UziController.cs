using UnityEngine;
using UnityEngine.VFX;

public class UziController : MonoBehaviour, IFireable
{
    private FireLine fireLine;
    public AudioSource audioSource;
    private Vector3 initalForward;
	[SerializeField] public float muzzleFlashDuration = 0.1f;
    [SerializeField] private float shotCooldownSeconds = 0.03f;
    public VisualEffect muzzleFlash;
	public Light muzzlePointFlashLight;
    public Light muzzleDirectionalFlashLight;
    private Cooldown cooldown;

    void Start()
    {
        fireLine = GetComponentInChildren<FireLine>();
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

        fireLine.Fire();
    }

    void DisableMuzzleFlashLight()
    {
        muzzlePointFlashLight.enabled = false;
        muzzleDirectionalFlashLight.enabled = false;
    }
}
