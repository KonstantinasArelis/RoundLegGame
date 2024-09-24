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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initalForward = transform.forward;
        initialRight = transform.right;
        camera = Camera.main.gameObject;
        gun = transform.Find("Gun").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Shoot();
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
        // move camera and player in the same direction
        transform.position += moveDirection;
        camera.transform.position += moveDirection;
    }

    private void Shoot()
    {
        float lastShotDifference = Time.time - lastShotTime;
        bool gunCooledDown = lastShotDifference >= shotCooldownSeconds;
        if (Input.GetKey(KeyCode.Space) && gunCooledDown)
        {
            GameObject bulletInstance = Instantiate(bullet, gun.transform.position + gun.transform.up, gun.transform.rotation);
            bulletInstance.GetComponent<Rigidbody>().linearVelocity = bulletInstance.transform.up * bulletSpeed;
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
