using System.Collections;
using UnityEngine;

public class PistolController : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 20.0f;
    [SerializeField] private float shotCooldownSeconds = 0.5f;
    [SerializeField] private float bulletDisposeSeconds = 2.0f;
    [SerializeField] private Vector3 bulletSpawnOffset; 

    private float lastShotTime = 0.0f;

    public void Fire()
    {
        float lastShotDifference = Time.time - lastShotTime;
        bool gunCooledDown = lastShotDifference >= shotCooldownSeconds;
        if (gunCooledDown)
        {
            // Apply the height offset to the bullet's starting position
            Vector3 bulletSpawnPosition = transform.position + bulletSpawnOffset;
            GameObject bulletInstance = Instantiate(bulletPrefab, bulletSpawnPosition, Quaternion.identity);
            bulletInstance.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
            lastShotTime = Time.time;
            StartCoroutine(DisposeBulletCoroutine(bulletInstance));
        }
    }

    private IEnumerator DisposeBulletCoroutine(GameObject bulletInstance)
    {
        yield return new WaitForSeconds(bulletDisposeSeconds);
        Destroy(bulletInstance);
    }
}
