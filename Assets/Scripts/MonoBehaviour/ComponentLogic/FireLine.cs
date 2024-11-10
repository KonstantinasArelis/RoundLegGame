using UnityEngine;

public class FireLine : MonoBehaviour, IFireable
{
    private Vector3 initalForward;
    LineRenderer lineRenderer; 
    private readonly float lineDistance = 10f;
    [SerializeField] private float lineDuration = 0.1f;
	
    void Start()
    {
    	lineRenderer = GetComponent<LineRenderer>(); 
    	initalForward = transform.forward;
    }
    
    public void Fire(float penetration, float knockbackForce, float Damage)
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
					// only target Enemy type
					// if (!hit.collider.CompareTag("Enemy")) return;
					if (hit.collider.TryGetComponent<IDamagable>(out var damagable))
					{
						damagable.TakeDamage(Damage);
						zombieController.TakeKnockback(knockbackForce);
					}
					if (hit.collider.TryGetComponent<Explosive>(out var explosive))
					{
						explosive.Explode();
					}
					if (hit.collider.TryGetComponent<Monster1Controller>(out Monster1Controller monster1Controller))
					{
						monster1Controller.TakeDamage(1);
					}
			}
    }
	
    void DisableLineRenderer()
    {
        lineRenderer.enabled = false;
    }
}

