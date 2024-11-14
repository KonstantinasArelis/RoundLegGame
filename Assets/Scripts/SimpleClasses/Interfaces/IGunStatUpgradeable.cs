public interface IGunStatUpgradeable
{
    int shotCooldownSecondsUpgradeCount {get; set;}
    int penetrationUpgradeCount {get; set;}
    int knockbackForceUpgradeCount {get; set;}
    int baseDamageUpgradeCount {get; set;}

    void IncreaseStat(GunStatPanelTypeEnum stat);
}
