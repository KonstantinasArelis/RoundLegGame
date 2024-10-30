using UnityEngine;

public class HealthbarController : MonoBehaviour
{
    private GameObject maxHealthBar;
    private GameObject currentHealthBar;
    private int maxHealth;
    private new Camera camera;
    // magic number that makes the healthbar decent width
    private float scale;

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

    // MUST call before use for instantiating properties
    public void SetupHealthbar(int currentHealth, int maxHealth, float scale)
    {
        this.scale = scale;
        this.maxHealth = maxHealth;
        currentHealthBar.transform.localScale = new Vector3(
            scale * currentHealth / maxHealth,
            scale / 6,
            currentHealthBar.transform.localScale.z
        );
        maxHealthBar.transform.localScale = new Vector3(
            scale,
            scale / 6,
            maxHealthBar.transform.localScale.z
        );
    }

    public void OnDamage(int damageAmount)
    {
        currentHealthBar.transform.localScale = new Vector3(
            currentHealthBar.transform.localScale.x - maxHealthBar.transform.localScale.x * damageAmount / maxHealth,
            currentHealthBar.transform.localScale.y,
            currentHealthBar.transform.localScale.z
        );
    }

    public void OnHeal(int healAmount)
    {
        currentHealthBar.transform.localScale = new Vector3(
            currentHealthBar.transform.localScale.x - maxHealthBar.transform.localScale.x * healAmount / maxHealth,
            currentHealthBar.transform.localScale.y,
            currentHealthBar.transform.localScale.z
        );
    }
}
