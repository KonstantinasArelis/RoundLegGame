using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private int selectedGun = 2;
    private new GameObject camera;
    private Vector3 initalForward;
    private Vector3 initialRight;
    private  PistolController pistolController;
    private  UziController uziController;
    private Vector3 positionOffsetFromCamera;
    Animator animator;

    public GameObject gunObject; 
    public GameObject uziObject; 
    public GameObject shotgunObject; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initalForward = transform.forward;
        initialRight = transform.right;
        camera = Camera.main.gameObject;
        pistolController = gameObject.FindComponentInChildWithTag<Transform>("Pistol").GetComponent<PistolController>();
        uziController = gameObject.FindComponentInChildWithTag<Transform>("Uzi").GetComponent<UziController>();
        positionOffsetFromCamera = transform.position - camera.transform.position;
        animator = GetComponent<Animator>();

        //default gun
        gunObject.SetActive(false);
        uziObject.SetActive(true); 
        shotgunObject.SetActive(false);  
    }

    // Update is called once per frame
    void Update()
    {
        MakeCameraKeepOffset();
        Move();
        Shoot();
        extraControls();
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

    private void extraControls()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            gunObject.SetActive(true);
            uziObject.SetActive(false);
            shotgunObject.SetActive(false);  
            selectedGun = 1;
        } else if (Input.GetKey(KeyCode.Alpha2))
        {
            gunObject.SetActive(false);
            uziObject.SetActive(true); 
            shotgunObject.SetActive(false);  
            selectedGun = 2;
        } else if (Input.GetKey(KeyCode.Alpha3))
        {
            gunObject.SetActive(false);
            uziObject.SetActive(false); 
            shotgunObject.SetActive(true);  
            selectedGun = 3;
        }
    }

    private void Shoot()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if(selectedGun == 1){
                pistolController.Fire();
            } else if(selectedGun == 2){
                uziController.Fire();
            }
        }
    }
}
