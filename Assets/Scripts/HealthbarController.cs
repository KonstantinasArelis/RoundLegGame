using UnityEngine;

public class HealthbarController : MonoBehaviour
{
    private GameObject maxHealthBar;
    private GameObject currentHealthBar;
    private new Camera camera;
    // magic number that makes the healthbar decent width
    private float xScaleFactor = 0.2f;

    void Awake()
    {
        // Awake is called before the ZombieController.Start()
        maxHealthBar = transform.Find("Max").gameObject;
        currentHealthBar = transform.Find("Current").gameObject;
        camera = Camera.main;
        GetComponent<Canvas>().worldCamera = camera;
    }

    void LateUpdate()
    {
        // face the camera
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
        // TODO: remove clipping over other objects
    }

    public void SetupHealthbar(int currentHealth, int maxHealth)
    {
        currentHealthBar.transform.localScale = new Vector3(
            currentHealth * xScaleFactor,
            currentHealthBar.transform.localScale.y,
            currentHealthBar.transform.localScale.z
        );
        maxHealthBar.transform.localScale = new Vector3(
            maxHealth * xScaleFactor,
            maxHealthBar.transform.localScale.y,
            maxHealthBar.transform.localScale.z
        );
    }

    public void OnDamage(int damageAmount)
    {
        currentHealthBar.transform.localScale = new Vector3(
            currentHealthBar.transform.localScale.x - damageAmount * xScaleFactor,
            currentHealthBar.transform.localScale.y,
            currentHealthBar.transform.localScale.z
        );
    }

    public void OnHeal(int healAmount)
    {
        currentHealthBar.transform.localScale = new Vector3(
            currentHealthBar.transform.localScale.x + healAmount * xScaleFactor,
            currentHealthBar.transform.localScale.y,
            currentHealthBar.transform.localScale.z
        );
    }
}
