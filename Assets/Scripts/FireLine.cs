using UnityEngine;
using UnityEngine.VFX;

public class FireLine : MonoBehaviour
{
    private Vector3 initalForward;
    LineRenderer lineRenderer; 
    private readonly float lineDistance = 10f;
    [SerializeField] public float lineDuration = 0.1f;
	
    void Start()
    {
    	lineRenderer = GetComponent<LineRenderer>(); 
    	initalForward = transform.forward;
    }
    
    public void Fire()
    {
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
					if (hit.collider.TryGetComponent<Monster1Controller>(out Monster1Controller Monster1Controller))
					{
						Monster1Controller.TakeDamage(1);
					}
			}
    }
	
    void DisableLineRenderer()
    {
        lineRenderer.enabled = false;
    }
}

