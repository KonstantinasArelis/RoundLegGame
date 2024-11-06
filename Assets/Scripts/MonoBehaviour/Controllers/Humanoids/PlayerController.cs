using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private UpgradeTypeEnum selectedGun = UpgradeTypeEnum.Uzi;

    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject xpBar;

    Vector3 healthBarOffset = new (-0.3f, 2f, 0f);
    Vector3 xpBarOffset = new (-0.3f, 2.15f, 0f);
    public HealthProvider healthProvider;
    public LevelProvider levelProvider;
    private new GameObject camera;
    private Vector3 initalForward;
    private Vector3 initialRight;
    private  PistolController pistolController;
    private  UziController uziController;
    private  ShotgunController shotgunController;
    [SerializeField] private Vector3 positionOffsetFromCamera = new (0, 8, -5);

    private TextMeshProUGUI levelText;

    private MainHudController mainHudController;

    public Cooldown damageCooldown = new (1.0f);
    Animator animator;

    public GameObject gunObject; 
    public GameObject uziObject; 
    public GameObject shotgunObject;

    private Dictionary<UpgradeTypeEnum, GameObject> gunToObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initalForward = transform.forward;
        initialRight = transform.right;
        camera = Camera.main.gameObject;
        pistolController = gameObject.FindComponentInChildWithTag<Transform>("Pistol").GetComponent<PistolController>();
        uziController = gameObject.FindComponentInChildWithTag<Transform>("Uzi").GetComponent<UziController>();
        shotgunController = gameObject.FindComponentInChildWithTag<Transform>("Shotgun").GetComponent<ShotgunController>();
        animator = GetComponent<Animator>();
        gunToObject = new () {
            {UpgradeTypeEnum.Pistol, gunObject},
            {UpgradeTypeEnum.Uzi, uziObject},
            {UpgradeTypeEnum.Shotgun, shotgunObject}
        };
        

        healthProvider = new HealthProvider(maxHealth: 10);

        healthBar = Instantiate(healthBar, transform.position, healthBar.transform.rotation);
        healthBar.GetComponent<QuantityBarController>().SetupQuantityBar(healthProvider.health, healthProvider.maxHealth, 0.2f);

        // TODO: not hardcore level progression
        levelProvider = new LevelProvider(xpNeededPerLevel: new int[]{20, 20, 20, 20});
        xpBar = Instantiate(xpBar, transform.position, xpBar.transform.rotation);
        xpBar.GetComponent<QuantityBarController>().SetupQuantityBar(0, levelProvider.XpNeededForCurrentLevel(), 0.2f);
        levelText = xpBar.transform.Find("Level").GetComponent<TextMeshProUGUI>();
        //default gun
        SelectGun(selectedGun);
    }

    void Awake()
    {
        mainHudController = GameObject.Find("MainHud").GetComponent<MainHudController>();
    }

    // Update is called once per frame
    void Update()
    {
        MakeStatBarsKeepOffset();
        MakeCameraKeepOffset();
        Shoot();
        ExtraControls();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void MakeStatBarsKeepOffset()
    {
        healthBar.transform.position = transform.position + healthBarOffset;
        xpBar.transform.position = transform.position + xpBarOffset;
    }

    private void MakeCameraKeepOffset()
    {
        // keep the same camera start offset from the player
        camera.transform.position = transform.position + positionOffsetFromCamera;
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
            SelectGun(UpgradeTypeEnum.Pistol);
        } else if (Input.GetKey(KeyCode.Alpha2))
        {
            SelectGun(UpgradeTypeEnum.Uzi);
        } else if (Input.GetKey(KeyCode.Alpha3))
        {
            SelectGun(UpgradeTypeEnum.Shotgun); 
        }
    }

    private void Shoot()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            FireSelectedGun();
        }
    }

    public void SelectGun(UpgradeTypeEnum gun)
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
            case UpgradeTypeEnum.Pistol:
                pistolController.Fire();
                break;
            case UpgradeTypeEnum.Uzi:
                uziController.Fire();
                break;
            case UpgradeTypeEnum.Shotgun:
                shotgunController.Fire();
                break;
        }
    }

    private void OnDeath()
    {
        Destroy(healthBar);
        Destroy(xpBar);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        if (!damageCooldown.IsReady()) return;
        healthBar.GetComponent<QuantityBarController>().Subtract(damage);
        healthProvider.TakeDamage(damage);
        if (healthProvider.IsDead())
        {
            OnDeath();
        }
    }

    public void GainXp(int xp)
    {
        bool didLevelUp = levelProvider.GainXp(xp);
        if (didLevelUp) OnLevelUp();
        else if (!levelProvider.IsMaxLevelReached()) xpBar.GetComponent<QuantityBarController>().Add(xp);
    }

    private void OnLevelUp()
    {
        int xpNeededForLevelUp = levelProvider.XpNeededForCurrentLevel();
        xpBar.GetComponent<QuantityBarController>().SetupQuantityBar(0, xpNeededForLevelUp, 0.2f);
        levelText.text = levelProvider.GetCurrentLevel().ToString();
        mainHudController.OnLevelUp();
    }
}
