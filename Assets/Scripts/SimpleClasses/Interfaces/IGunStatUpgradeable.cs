public interface IGunStatUpgradeable
{
    /*
    public float UpgradeShotCooldownSeconds(float shotCooldownSeconds);
    public float UpgradePenetration(float penetration);
    public float UpgradeKnockbackForce(float knockbackForce);
    public float UpgradebaseDamage(float baseDamage);
    */
    //public void UpgradeStat(string stat);
    float shotCooldownSeconds {get; set;}
    float penetration {get; set;}
    float knockbackForce {get; set;}
    float baseDamage {get; set;}
}
