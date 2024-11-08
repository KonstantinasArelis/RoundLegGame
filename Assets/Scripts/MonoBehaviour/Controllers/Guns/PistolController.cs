using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class PistolController : MonoBehaviour
{
    private FireLine fireLine;
    
    private Vector3 initalForward;
	[SerializeField] public float muzzleFlashDuration = 0.1f;
    [SerializeField] private float shotCooldownSeconds = 0.03f;
    [SerializeField] private int penetration = 3;
    private float lastShotTime = 0.0f;
    public VisualEffect muzzleFlash;
	public Light muzzlePointFlashLight;
    public Light muzzleDirectionalFlashLight;
    void Start()
    {
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

        fireLine.Fire(penetration);
    }

    void DisableMuzzleFlashLight()
    {
        muzzlePointFlashLight.enabled = false;
        muzzleDirectionalFlashLight.enabled = false;
    }
}
