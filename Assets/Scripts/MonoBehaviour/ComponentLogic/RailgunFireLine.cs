using UnityEngine;

public class RailgunFireLine : MonoBehaviour
{
    private Vector3 initalForward;
    LineRenderer lineRenderer; 
    private float lineDistance = 5f;
    [SerializeField] private float lineDuration = 0.3f;
	public float maxAngleDeviation = 30f;
	public Vector3 direction;
	public Vector3 lastTransformDireciton;
	public Vector3 endPoint;
	public Vector3 position;

	
	public int lengthOfLineRenderer = 2;
	public Vector3[] points;
    void Start()
    {
    	lineRenderer = GetComponent<LineRenderer>(); 
    	initalForward = transform.forward;
		lineRenderer.positionCount = lengthOfLineRenderer;
		points = new Vector3[1000];
    }
    
    public void Fire(float penetration, float knockbackForce, float damage, bool firstShot)
    {
			if(lengthOfLineRenderer>=1000){
				//strange bug happens if this is not here
				lengthOfLineRenderer=2;
			}
			
			//Debug.Log("points: " + lengthOfLineRenderer);
			direction = transform.forward;

			float fixedY = 0.0f; // Replace with your desired height value
    		direction.y = fixedY; 

			if(firstShot == false)
			{
				// Generate random angles for x and y axes
				//float angleX = Random.Range(-maxAngleDeviation, maxAngleDeviation);
				float angleY = Random.Range(-maxAngleDeviation, maxAngleDeviation);
				float angleX = 0;
				// Rotate the direction vector by the random angles
				direction = Quaternion.Euler(angleX, angleY, 0f) * direction;
			}

			if(firstShot == false)
			{
				position = endPoint;
				endPoint = position + direction * lineDistance;
				lengthOfLineRenderer++;
				points[lengthOfLineRenderer-1] = endPoint;
			} else {
				position = transform.position;
				endPoint = position + direction * lineDistance;
				points[0] = position;
				points[1] = endPoint;
			}
			
			//lineRenderer.SetPosition(0, position);
			//lineRenderer.SetPosition(1, endPoint); 
			lineRenderer.positionCount = lengthOfLineRenderer;
			lineRenderer.SetPositions(points);
			lineRenderer.enabled = true;
			Invoke(nameof(DisableLineRenderer), lineDuration);
			
			if (Physics.Raycast(position, direction, out RaycastHit hit, lineDistance))
			{
					var hitDistance = (hit.point - position).sqrMagnitude;
					var maxDistance = (endPoint - position).sqrMagnitude;
					if (hitDistance < maxDistance)
					{
						// limit the "bullet" to the object it hit
						//lineRenderer.SetPosition(1, hit.point);
						if(firstShot == false)
						{
							endPoint = hit.point;
							points[lengthOfLineRenderer-1] = endPoint;
						} else {
							endPoint = hit.point;
							points[0] = position;
							points[1] = endPoint;
						}
					}
					// only target Enemy type
					// if (!hit.collider.CompareTag("Enemy")) return;
					if (hit.collider.TryGetComponent<IDamagable>(out var damagable))
					{
						damagable.TakeDamage(damage);
						if(penetration>1)
						{
							penetration = penetration-1;
							var lastFractureIndex = lengthOfLineRenderer;

							this.Fire(penetration, knockbackForce, damage, false);
							lengthOfLineRenderer++;
							points[lengthOfLineRenderer-1] = points[lastFractureIndex-1];
							
							this.Fire(penetration, knockbackForce, damage, false);
							lengthOfLineRenderer++;
							points[lengthOfLineRenderer-1] = points[lastFractureIndex-1];

							this.Fire(penetration, knockbackForce, damage, false);
							lengthOfLineRenderer++;
							points[lengthOfLineRenderer-1] = points[lastFractureIndex-1];

							if(firstShot==true){
								lengthOfLineRenderer = 2;
							}
						}
					}
					if (hit.collider.TryGetComponent<IKnockable>(out var knockable))
					{
						knockable.TakeKnockback(knockbackForce, transform.position);
					}
					if (hit.collider.TryGetComponent<Explosive>(out var explosive))
					{
						explosive.Explode();
					}
			}
			
			
    }
	
    void DisableLineRenderer()
    {
        lineRenderer.enabled = false;
    }
}
