using UnityEngine;
using UnityEngine.VFX;

public class UziController : MonoBehaviour
{
    	private Vector3 initalForward;
    	LineRenderer lineRenderer; 
    	float lineDistance = 10f;
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
    	if (gunCooledDown){
			muzzleFlashLight.enabled = true;
			Invoke("DisableMuzzleFlashLight", muzzleFlashDuration); 

			muzzleFlash.Play();
        	lastShotTime = Time.time;
        	Vector3 direction = transform.forward;
	    	Vector3 endPoint = transform.position + direction * lineDistance;  
	    	lineRenderer.SetPosition(0, transform.position);
	    	lineRenderer.SetPosition(1, endPoint); 
	    	lineRenderer.enabled = true;
	    	Invoke("DisableLineRenderer", lineDuration);
	    	
	    	RaycastHit hit;
		if (Physics.Raycast(transform.position, direction, out hit)) {
			Debug.Log("Hit: " + hit.collider.name);  
			if (hit.collider.TryGetComponent<ZombieController>(out ZombieController zombieController)) { 
			    zombieController.TakeDamage(1); 
			}
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
