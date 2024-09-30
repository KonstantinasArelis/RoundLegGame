using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    private new GameObject camera;
    private Vector3 initalForward;
    private Vector3 initialRight;
    private  GunController gunController;
    private Vector3 positionOffsetFromCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initalForward = transform.forward;
        initialRight = transform.right;
        camera = Camera.main.gameObject;
        gunController = gameObject.FindComponentInChildWithTag<Transform>("Gun").GetComponent<GunController>();
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
        // keep the same camera start offset from the player
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
        if (Input.GetKey(KeyCode.Space))
        {
            gunController.Fire();
        }
    }
}
