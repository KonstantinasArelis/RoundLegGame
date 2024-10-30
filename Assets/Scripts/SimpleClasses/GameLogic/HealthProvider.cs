public class HealthProvider
{
  public int health;
  public int maxHealth;

  public HealthProvider(int health, int maxHealth)
  {
      this.health = health;
      this.maxHealth = maxHealth;
  }
  public HealthProvider(int maxHealth)
  {
      this.maxHealth = maxHealth;
      health = maxHealth;
  }

  public void Heal(int healAmount)
  {
      health += healAmount;
      EnsureMaxHealthCap();
  }

  public void TakeDamage(int damage)
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
