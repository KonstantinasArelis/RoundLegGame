using UnityEngine;
using UnityEngine.VFX;

public class LineFire : MonoBehaviour
{
    	private Vector3 initalForward;
    	LineRenderer lineRenderer; 
    	private readonly float lineDistance = 10f;
    	[SerializeField] public float lineDuration = 0.1f;
		[SerializeField] public float muzzleFlashDuration = 0.1f;
    	[SerializeField] private float shotCooldownSeconds = 0.03f;
    	private float lastShotTime = 0.0f;
    	public VisualEffect muzzleFlash;
		public Light muzzleFlashLight;

    void Start()
    {
		muzzleFlashLight.enabled = false;
    	lineRenderer = GetComponent<LineRenderer>(); 
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
			
			muzzleFlashLight.enabled = true;
			Invoke(nameof(DisableMuzzleFlashLight), muzzleFlashDuration); 

			muzzleFlash.Play();
					lastShotTime = Time.time;
					Vector3 direction = transform.forward;
			Vector3 endPoint = transform.position + direction * lineDistance;  
			lineRenderer.SetPosition(0, transform.position);
			lineRenderer.SetPosition(1, endPoint); 
			lineRenderer.enabled = true;
			Invoke(nameof(DisableLineRenderer), lineDuration);

			if (Physics.Raycast(transform.position, direction, out RaycastHit hit, lineDistance))
			{
					var hitDistance = (hit.point - transform.position).sqrMagnitude;
					var maxDistance = (endPoint - transform.position).sqrMagnitude;
					if (hitDistance < maxDistance)
					{
						// limit the "bullet" to the object it hit
						lineRenderer.SetPosition(1, hit.point);
					}
					if (hit.collider.TryGetComponent<ZombieController>(out ZombieController zombieController))
					{
						zombieController.TakeDamage(1);
					}
			}
    }
    
	void DisableMuzzleFlashLight()
    {
        muzzleFlashLight.enabled = false;
    }
	
    void DisableLineRenderer()
    {
        lineRenderer.enabled = false;
    }
}

