using UnityEngine;
using UnityEngine.VFX;

public class UziController : MonoBehaviour, IFireable, IGunStatUpgradeable
{
    private FireLine fireLine;
    public AudioSource audioSource;
    private Vector3 initalForward;
    [SerializeField] public float muzzleFlashDuration = 0.1f;


    [SerializeField] public float startingShotCooldownSeconds = 0.08f;
    [SerializeField] public float startingPenetration = 2f;
    [SerializeField] public float startingKnockbackForce = 1f;
    [SerializeField] public float startingBaseDamage = 1f;

    public float shotCooldownSeconds {get; set;}
    public float penetration {get; set;}
    public float knockbackForce {get; set;}
    public float baseDamage {get; set;}

    public int shotCooldownSecondsUpgradeCount {get; set;}
    public int penetrationUpgradeCount {get; set;}
    public int knockbackForceUpgradeCount {get; set;}
    public int baseDamageUpgradeCount {get; set;}
    
    private float lastShotTime = 0.0f;
    public VisualEffect muzzleFlash;
	public Light muzzlePointFlashLight;
    public Light muzzleDirectionalFlashLight;
    private Cooldown cooldown;

    void Start()
    {
        this.shotCooldownSeconds = startingShotCooldownSeconds;
        this.penetration = startingPenetration;
        this.knockbackForce = startingKnockbackForce;
        this.baseDamage = startingBaseDamage;

        this.shotCooldownSecondsUpgradeCount = 0;
        this.penetrationUpgradeCount = 0;
        this.knockbackForceUpgradeCount = 0;
        this.baseDamageUpgradeCount = 0;
        

        fireLine = GetComponentInChildren<FireLine>();
        audioSource = GetComponent<AudioSource>();
        muzzlePointFlashLight.enabled = false;
        muzzleDirectionalFlashLight.enabled = false;
    	initalForward = transform.forward;
        cooldown = new (shotCooldownSeconds);
    }

    public void Fire()
    {
        if (!cooldown.IsReady()) return;
		
		muzzlePointFlashLight.enabled = true;
        muzzleDirectionalFlashLight.enabled = true;
		Invoke(nameof(DisableMuzzleFlashLight), muzzleFlashDuration); 
		muzzleFlash.Play();
        audioSource.Play();

        fireLine.Fire(penetration, knockbackForce, baseDamage);
    }

    void DisableMuzzleFlashLight()
    {
        muzzlePointFlashLight.enabled = false;
        muzzleDirectionalFlashLight.enabled = false;
    }

    public void IncreaseStat(GunStatPanelTypeEnum stat){

        switch(stat)
        {
            case GunStatPanelTypeEnum.BaseDamage:
                baseDamageUpgradeCount++;
                baseDamage = startingBaseDamage + baseDamageUpgradeCount*2;
            break;
            case GunStatPanelTypeEnum.ShotCooldownSeconds:
                shotCooldownSecondsUpgradeCount++;
                shotCooldownSeconds = startingShotCooldownSeconds / shotCooldownSecondsUpgradeCount;
                cooldown = new (shotCooldownSeconds);
            break;
            case GunStatPanelTypeEnum.Penetration:
                penetrationUpgradeCount++;
                penetration = startingPenetration + penetrationUpgradeCount;
            break;
            case GunStatPanelTypeEnum.Knockback:
                knockbackForceUpgradeCount++;
                knockbackForce = startingKnockbackForce + knockbackForceUpgradeCount*2;
            break;
        }
        Debug.Log("upgraded: " + stat);
    }
}
