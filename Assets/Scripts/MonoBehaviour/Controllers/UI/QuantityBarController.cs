using UnityEngine;

public class QuantityBarController : MonoBehaviour
{
    private GameObject maxBar;
    private GameObject currentBar;
    private float max;
    private new Camera camera;
    // magic number that makes the healthbar decent width
    private float scale;

    void Awake()
    {
        // Awake is called before the ZombieController.Start()
        maxBar = transform.Find("Max").gameObject;
        currentBar = transform.Find("Current").gameObject;
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
    public void SetupQuantityBar(float current, float max, float scale)
    {
        this.scale = scale;
        this.max = max;
        currentBar.transform.localScale = new Vector3(
            scale * current / max,
            // TODO: fix the magic number
            scale / 6,
            currentBar.transform.localScale.z
        );
        maxBar.transform.localScale = new Vector3(
            scale,
            scale / 6,
            maxBar.transform.localScale.z
        );
    }

    public void Subtract(float amount)
    {
        currentBar.transform.localScale = new Vector3(
            currentBar.transform.localScale.x - maxBar.transform.localScale.x * amount / max,
            currentBar.transform.localScale.y,
            currentBar.transform.localScale.z
        );
    }

    public void Add(float amount)
    {
        currentBar.transform.localScale = new Vector3(
            currentBar.transform.localScale.x + maxBar.transform.localScale.x * amount / max,
            currentBar.transform.localScale.y,
            currentBar.transform.localScale.z
        );
    }
}
