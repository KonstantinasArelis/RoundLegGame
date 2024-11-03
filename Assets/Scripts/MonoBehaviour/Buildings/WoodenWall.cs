using UnityEngine;

public class WoodenWall : MonoBehaviour, IDamagable
{
    private HealthProvider healthProvider;
    private Cooldown damageCooldown = new (0.5f);

    void Start()
    {
        healthProvider = new HealthProvider(maxHealth: 3);

    }

    public void TakeDamage(int damage)
    {
        if (!damageCooldown.IsReady()) return;
        healthProvider.TakeDamage(damage);
        if (healthProvider.IsDead())
        {
            Destroy(gameObject);
        }
    }
}
