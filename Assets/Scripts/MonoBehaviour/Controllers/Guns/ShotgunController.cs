using UnityEngine;
using UnityEngine.VFX;
using System.Collections;


public class ShotgunController : MonoBehaviour
{
    private FireLine[] fireLines;

    private Vector3 initalForward;
	[SerializeField] public float muzzleFlashDuration = 0.1f;
    [SerializeField] private float shotCooldownSeconds = 0.03f;
    [SerializeField] private int penetration = 1;
    private float lastShotTime = 0.0f;
    public VisualEffect muzzleFlash;
	public Light muzzlePointFlashLight;
    public Light muzzleDirectionalFlashLight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            fireLine.Fire(penetration);
        }
    }

    void DisableMuzzleFlashLight()
    {
        muzzlePointFlashLight.enabled = false;
        muzzleDirectionalFlashLight.enabled = false;
    }
}
