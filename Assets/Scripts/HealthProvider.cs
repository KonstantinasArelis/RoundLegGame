using UnityEngine;
using UnityEngine.Events;

public class HealthProvider : MonoBehaviour
{
    public int health;
    public int maxHealth;

    public UnityEvent<int> DamageTaken;
    

    public void Heal(int healAmount)
    {
        health += healAmount;
        EnsureMaxHealthCap();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        DamageTaken.Invoke(damage);
    }

    public bool IsDead()
    {
        return health <= 0;
    }

    private void EnsureMaxHealthCap()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
}
