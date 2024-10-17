using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    private new GameObject camera;
    private Vector3 initalForward;
    private Vector3 initialRight;
    private  GunController gunController;
    private  UziController uziController;
    private Vector3 positionOffsetFromCamera;
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initalForward = transform.forward;
        initialRight = transform.right;
        camera = Camera.main.gameObject;
        gunController = gameObject.FindComponentInChildWithTag<Transform>("Gun").GetComponent<GunController>();
        uziController = gameObject.FindComponentInChildWithTag<Transform>("Gun").GetComponent<UziController>();
        positionOffsetFromCamera = transform.position - camera.transform.position;
        animator = GetComponent<Animator>();
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
        animator.SetBool("isWalking", true);
            moveDirection += initalForward;
        }
        if (Input.GetKey(KeyCode.S))
        {
        animator.SetBool("isWalking", true);
            moveDirection -= initalForward;
        }
        if (Input.GetKey(KeyCode.A))
        {
        animator.SetBool("isWalking", true);
            moveDirection -= initialRight;
        }
        if (Input.GetKey(KeyCode.D))
        {
        animator.SetBool("isWalking", true);
            moveDirection += initialRight;
        }
        if (moveDirection == Vector3.zero)
        {
        	animator.SetBool("isWalking", false);
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
            //gunController.Fire();
            uziController.Fire();
        }
    }
}
