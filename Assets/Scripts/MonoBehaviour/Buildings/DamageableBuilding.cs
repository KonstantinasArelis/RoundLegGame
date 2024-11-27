using UnityEngine;

public class DamageableBuilding : MonoBehaviour, IDamagable
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float damageCooldownTime;
    private Cooldown damageCooldown;
    private HealthProvider healthProvider;

    void Start()
    {
        healthProvider = new (maxHealth);
        damageCooldown = new (damageCooldownTime);
    }

    public void TakeDamage(float damage)
    {
        if (!damageCooldown.IsReady()) return;
        healthProvider.TakeDamage(damage);
        if (healthProvider.IsDead())
        {
            Destroy(gameObject);
        }
    }
}
