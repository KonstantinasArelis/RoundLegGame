using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamagable
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject playerBar;
    private GameObject healthBar, xpBar;

    // picked values from observation
    private Directions cameraBoundaryFromGround = new() {
        front = -16f,
        right = -12f,
        back = 0,
        left = 12f
    };

    [SerializeField] private int maxHealth;

    Vector3 playerBarOffset = new (-0.3f, 2f, 0f);
    public HealthProvider healthProvider;
    public LevelProvider levelProvider;
    private new GameObject camera;
    private Vector3 initalForward;
    private Vector3 initialRight;
    [SerializeField] private Vector3 positionOffsetFromCamera = new (0, 8, -5);

    private TextMeshProUGUI levelText;

    private MainHudController mainHudController;

    private GameObject currentGun;

    public Cooldown damageCooldown = new (1.0f);

    // for testing
    private  UpgradeData[] availableUpgrades;

    private GameObject ground;
    private Directions groundBoundary;

    Animator animator;

    public GameObject gunObject; 
    public GameObject uziObject; 
    public GameObject shotgunObject;
    public IGunStatUpgradeable selectedGunController;

    public GameObject creditsPanel;

    // private int playerScore; // Variable to track the player's score

    Dictionary<UpgradeTypeEnum, GameObject> gunToObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initalForward = transform.forward;
        initialRight = transform.right;
        camera = Camera.main.gameObject;
        animator = GetComponent<Animator>();
        gunToObject = new () {
            {UpgradeTypeEnum.Pistol, gunObject},
            {UpgradeTypeEnum.Uzi, uziObject},
            {UpgradeTypeEnum.Shotgun, shotgunObject}
        };
        

        healthProvider = new HealthProvider(maxHealth);
        levelProvider = new LevelProvider(Maps.xpNeededPerLevel);

        playerBar = Instantiate(playerBar, transform.position, playerBar.transform.rotation);
        xpBar = playerBar.transform.Find("Xpbar").gameObject;
        healthBar = playerBar.transform.Find("Healthbar").gameObject;
        xpBar.GetComponent<QuantityBarController>().SetupQuantityBar(0, levelProvider.XpNeededForCurrentLevel());
        healthBar.GetComponent<QuantityBarController>().SetupQuantityBar(healthProvider.health, healthProvider.maxHealth);
        levelText = xpBar.transform.Find("Level").GetComponent<TextMeshProUGUI>();
        
        ground = GameObject.FindWithTag("Ground");
        groundBoundary = Utility.GetCollidableObjectBoundaries(ground);


        // for testing
        availableUpgrades = Utility.LoadResourcesToArray<UpgradeData>("ScriptableObjects/Upgrades");
    }

    void Awake()
    {
        mainHudController = GameObject.Find("MainHud").GetComponent<MainHudController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!PauseMenuManager.isPaused)
        {
            MakeStatBarsKeepOffset();
            MakeCameraKeepOffset();
            Shoot();
            ExtraControls();
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    private void MakeStatBarsKeepOffset()
    {
        playerBar.transform.position = transform.position + playerBarOffset;
    }

    private void MakeCameraKeepOffset()
    {
        // keep the same camera start offset from the player
        camera.transform.position = transform.position + positionOffsetFromCamera;
        Directions clampedCameraPosition = groundBoundary + cameraBoundaryFromGround;
        camera.transform.position = new Vector3(
            Mathf.Clamp(camera.transform.position.x, clampedCameraPosition.left, clampedCameraPosition.right),
            camera.transform.position.y,
            Mathf.Clamp(camera.transform.position.z, clampedCameraPosition.back, clampedCameraPosition.front)
        );
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

    private GameObject GetGun(UpgradeTypeEnum upgradeType)
    {
        // try to get the gun else it is an upgrade
        if (Maps.gunToTag.TryGetValue(upgradeType, out string gunName))
        {
            return gameObject.FindComponentInChildWithTag<Transform>(gunName).gameObject;
        }
        return null;
    }

    private IFireable GetFireable(GameObject gun) => gun.GetComponent<IFireable>();

    private void Shoot()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            GetFireable(currentGun)?.Fire();
        }
    }

    public void SelectUpgrade(UpgradeData upgrade)
    {
        GameObject gunObject = GetGun(upgrade.type);
        if (gunObject == null)
        {
            SelectPassiveUpgrade(upgrade);
            return;
        }
        SelectGun(gunObject);
    }

    private void SelectGun(GameObject gunObject)
    {
        if (currentGun != null) currentGun.SetActive(false);
        currentGun = gunObject;
        selectedGunController = currentGun.GetComponent<IGunStatUpgradeable>();
        currentGun.SetActive(true);
        mainHudController.AnimateGunStatPanelUpdate();
    }

    private void SelectPassiveUpgrade(UpgradeData upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeTypeEnum.KillerOrb:
                var orb = Instantiate(upgrade.prefab, transform.position, transform.rotation);
                orb.GetComponent<FlyingOrb>().orbitTarget = transform;
            break;
            case UpgradeTypeEnum.AxeThrower:
                var axeThrower = Instantiate(upgrade.prefab, transform.position, transform.rotation);
                axeThrower.GetComponent<AbstractThrower>().spawnTransform = transform;
            break;
            case UpgradeTypeEnum.MolotovThrower:
                var molotovThrower = Instantiate(upgrade.prefab, transform.position, transform.rotation);
                molotovThrower.GetComponent<AbstractThrower>().spawnTransform = transform;
            break;
        }
    }


    private void ExtraControls()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            SelectUpgrade(availableUpgrades.Where(u => u.type == UpgradeTypeEnum.Pistol).First());
        } else if (Input.GetKey(KeyCode.Alpha2))
        {
            SelectUpgrade(availableUpgrades.Where(u => u.type == UpgradeTypeEnum.Uzi).First());
        } else if (Input.GetKey(KeyCode.Alpha3))
        {
            SelectUpgrade(availableUpgrades.Where(u => u.type == UpgradeTypeEnum.Shotgun).First()); 
        }
    }

    private void OnDeath()
    {
        Destroy(healthBar);
        Destroy(xpBar);
        Destroy(gameObject);

        EndMenuManager endMenuManager = GameObject.Find("/EndMenuManager")?.GetComponent<EndMenuManager>();

        if (endMenuManager)
            endMenuManager.ShowEndMenu(mainHudController.GetScore());
    }

    public async void TakeDamage(float damage)
    {
        if (!damageCooldown.IsReady()) return;
        healthProvider.TakeDamage(damage);
        await healthBar.GetComponent<QuantityBarController>().SetCurrent(healthProvider.health);
        if (healthProvider.IsDead())
        {
            OnDeath();
        }
    }

    public async void GainXp(int xp)
    {
        bool didLevelUp = levelProvider.GainXp(xp);
        if (didLevelUp) OnLevelUp();
        else if (!levelProvider.IsMaxLevelReached())
            await xpBar.GetComponent<QuantityBarController>().SetCurrent(levelProvider.GetCurrentXp());
    }

    private void OnLevelUp()
    {
        int xpNeededForLevelUp = levelProvider.XpNeededForCurrentLevel();
        xpBar.GetComponent<QuantityBarController>().SetupQuantityBar(0, xpNeededForLevelUp);
        Tweens.Pop(levelText.transform, 1.2f, 0.2f);
        levelText.text = levelProvider.GetCurrentLevel().ToString();
        mainHudController.OnLevelUp();
    }

    public void UpgradeSelectedGun(GunStatPanelTypeEnum statName)
    {
        selectedGunController.IncreaseStat(statName);
        /*
        switch (statName)
        {
            case "shotCooldownSeconds":
                selectedGunController.shotCooldownSecondsUpgradeCount++;
                break;
            case "penetration":
                selectedGunController.penetrationUpgradeCount++;
                break;
            case "knockbackForce":
                selectedGunController.knockbackForceUpgradeCount++;
                break;
            case "baseDamage":
                selectedGunController.baseDamageUpgradeCount++;
                break;
        }
        */
    }
}
