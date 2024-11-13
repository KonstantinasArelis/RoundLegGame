using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamagable
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject xpBar;

    [SerializeField] private int maxHealth;

    Vector3 healthBarOffset = new (-0.3f, 2f, 0f);
    Vector3 xpBarOffset = new (-0.3f, 2.15f, 0f);
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
    Animator animator;

    public GameObject gunObject; 
    public GameObject uziObject; 
    public GameObject shotgunObject;
    public IGunStatUpgradeable currentGunStatUpgradable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initalForward = transform.forward;
        initialRight = transform.right;
        camera = Camera.main.gameObject;
        animator = GetComponent<Animator>();
        

        healthProvider = new HealthProvider(maxHealth);

        healthBar = Instantiate(healthBar, transform.position, healthBar.transform.rotation);
        healthBar.GetComponent<QuantityBarController>().SetupQuantityBar(healthProvider.health, healthProvider.maxHealth);

        // TODO: not hardcore level progression
        levelProvider = new LevelProvider(Maps.xpNeededPerLevel);
        xpBar = Instantiate(xpBar, transform.position, xpBar.transform.rotation);
        xpBar.GetComponent<QuantityBarController>().SetupQuantityBar(0, levelProvider.XpNeededForCurrentLevel());
        levelText = xpBar.transform.Find("Level").GetComponent<TextMeshProUGUI>();

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
        currentGunStatUpgradable = currentGun.GetComponent<IGunStatUpgradeable>();
        currentGun.SetActive(true);
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

    public void UpgradeSelectedGun(string statName)
    {
        switch (statName)
        {
            case "shotCooldownSeconds":
                currentGunStatUpgradable.shotCooldownSeconds /= 2f;
                break;
            case "penetration":
                currentGunStatUpgradable.penetration += 1f;
                break;
            case "knockbackForce":
                currentGunStatUpgradable.knockbackForce += 5f;
                break;
            case "baseDamage":
                currentGunStatUpgradable.baseDamage += 1f;
                break;
        }

        Debug.Log("Upgraded: " + statName);
    }
}
