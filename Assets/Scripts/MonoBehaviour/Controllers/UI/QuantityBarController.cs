using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class QuantityBarController : MonoBehaviour
{
    private GameObject maxBar;
    private GameObject currentBar;
    private float max;
    private float scale;
    private new Camera camera;

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
    // scale is a magic value
    public void SetupQuantityBar(float current, float max, float scale=0.2f)
    {
        this.max = max;
        this.scale = scale;
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

    public Task SetCurrent(float current)
    {
        return currentBar.transform.DOScale(new Vector3(
            scale * current / max,
            currentBar.transform.localScale.y,
            currentBar.transform.localScale.z
        ), 0.1f).AsyncWaitForCompletion();
    }
}
