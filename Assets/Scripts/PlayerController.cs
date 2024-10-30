using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private GunEnum selectedGun = GunEnum.Uzi;

    [SerializeField] private GameObject healthbar;
    public HealthProvider healthProvider;
    private new GameObject camera;
    private Vector3 initalForward;
    private Vector3 initialRight;
    private  PistolController pistolController;
    private  UziController uziController;
    private  ShotgunController shotgunController;
    private Vector3 positionOffsetFromCamera;

    public Cooldown damageCooldown;
    Animator animator;

    public GameObject gunObject; 
    public GameObject uziObject; 
    public GameObject shotgunObject;

    private enum GunEnum
    {
        Pistol,
        Uzi,
        Shotgun
    }

    private Dictionary<GunEnum, GameObject> gunToObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        damageCooldown = new Cooldown(0.5f);
        initalForward = transform.forward;
        initialRight = transform.right;
        camera = Camera.main.gameObject;
        pistolController = gameObject.FindComponentInChildWithTag<Transform>("Pistol").GetComponent<PistolController>();
        uziController = gameObject.FindComponentInChildWithTag<Transform>("Uzi").GetComponent<UziController>();
        shotgunController = gameObject.FindComponentInChildWithTag<Transform>("Shotgun").GetComponent<ShotgunController>();
        positionOffsetFromCamera = transform.position - camera.transform.position;
        animator = GetComponent<Animator>();
        gunToObject = new () {
            {GunEnum.Pistol, gunObject},
            {GunEnum.Uzi, uziObject},
            {GunEnum.Shotgun, shotgunObject}
        };

        

        healthProvider = new HealthProvider(health: 10, maxHealth: 10);

        healthbar = Instantiate(healthbar, transform.position, healthbar.transform.rotation);
        healthbar.GetComponent<HealthbarController>().SetupHealthbar(healthProvider.health, healthProvider.maxHealth, 0.2f);

        //default gun
        SelectGun(selectedGun);
    }

    // Update is called once per frame
    void Update()
    {
        MakeHealthBarKeepOffset();
        MakeCameraKeepOffset();
        Move();
        Shoot();
        ExtraControls();
    }

    private void MakeHealthBarKeepOffset()
    {
        // keep the same healthbar start position from the player
        healthbar.transform.position = transform.position + new Vector3(0.3f, -1.7f, -9.5f);
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

    private void ExtraControls()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            SelectGun(GunEnum.Pistol);
        } else if (Input.GetKey(KeyCode.Alpha2))
        {
            SelectGun(GunEnum.Uzi);
        } else if (Input.GetKey(KeyCode.Alpha3))
        {
            SelectGun(GunEnum.Shotgun); 
        }
    }

    private void Shoot()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            FireSelectedGun();
        }
    }

    private void SelectGun(GunEnum gun)
    {
        foreach (var g in gunToObject)
        {
            g.Value.SetActive(false);
        }
        gunToObject[gun].SetActive(true);
        selectedGun = gun;
    }

    private void FireSelectedGun()
    {
        switch (selectedGun)
        {
            case GunEnum.Pistol:
                pistolController.Fire();
                break;
            case GunEnum.Uzi:
                uziController.Fire();
                break;
            case GunEnum.Shotgun:
                shotgunController.Fire();
                break;
        }
    }

    private void OnDeath()
    {
        Destroy(healthbar);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        if (!damageCooldown.IsReady()) return;
        healthbar.GetComponent<HealthbarController>().OnDamage(damage);
        healthProvider.TakeDamage(damage);
        if (healthProvider.IsDead())
        {
            OnDeath();
        }
    }
}
