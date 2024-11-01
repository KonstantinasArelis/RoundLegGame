using UnityEngine;

public class MuzzleLight : MonoBehaviour
{
    private Light pointLight;

    void Start()
    {
        pointLight = GetComponent<Light>();
        Invoke("DisableLight", 0.1f); // Disable the light after 1 second
    }

    void DisableLight()
    {
        pointLight.enabled = false; 
        // Alternatively, you can destroy the light completely:
        // Destroy(gameObject); 
    }
}