using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float bulletSpeed = 20.0f;
    [SerializeField] private float shotCooldownSeconds = 0.5f;
    [SerializeField] private float bulletDisposeSeconds = 2.0f;
    [SerializeField] private GameObject bullet;
    private new GameObject camera;
    private Vector3 initalForward;
    private Vector3 initialRight;
    private GameObject gun;
    private float lastShotTime = 0.0f;
    private Vector3 positionOffsetFromCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initalForward = transform.forward;
        initialRight = transform.right;
        camera = Camera.main.gameObject;
        gun = transform.Find("Gun").gameObject;
        positionOffsetFromCamera = transform.position - camera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MakeCameraKeepOffset();
        Move();
        Shoot();
    }

    private void MakeCameraKeepOffset()
    {
        // keep offset that the distance when placer in scene
        camera.transform.position = transform.position - positionOffsetFromCamera;
    }

    private void Move()
    {
        Vector3 moveDirection = Vector3.zero;
        // move in x, z plane using wasd
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += initalForward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= initalForward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= initialRight;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += initialRight;
        }
        if (moveDirection == Vector3.zero)
        {
            // player not moving
            return;
        }
        moveDirection = moveDirection.normalized;
        // make player look at the direction it's moving
        transform.rotation = Quaternion.LookRotation(moveDirection, transform.up);
        moveDirection *= speed * Time.deltaTime;
        transform.position += moveDirection;
    }

    private void Shoot()
    {
        float lastShotDifference = Time.time - lastShotTime;
        bool gunCooledDown = lastShotDifference >= shotCooldownSeconds;
        if (Input.GetKey(KeyCode.Space) && gunCooledDown)
        {
            GameObject bulletInstance = Instantiate(bullet, gun.transform.position + gun.transform.up, gun.transform.rotation);
            bulletInstance.GetComponent<Rigidbody>().AddForce(gun.transform.up * bulletSpeed, ForceMode.Impulse);
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
