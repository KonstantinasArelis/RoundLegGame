public class HealthProvider
{
  public float health;
  public float maxHealth;

  public HealthProvider(float health, float maxHealth)
  {
      this.health = health;
      this.maxHealth = maxHealth;
  }
  public HealthProvider(float maxHealth)
  {
      this.maxHealth = maxHealth;
      health = maxHealth;
  }

  public void Heal(float healAmount)
  {
      health += healAmount;
      EnsureMaxHealthCap();
  }

  public void TakeDamage(float damage)
  {
      health -= damage;
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
