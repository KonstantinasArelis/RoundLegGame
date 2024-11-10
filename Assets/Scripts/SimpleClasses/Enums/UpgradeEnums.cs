using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;


public enum UpgradeTypeEnum
{
    Pistol,
    Uzi,
    Shotgun
}

public enum UpgradeTierEnum
{
    Starting,
    First,
    Second,
    Third
}

[Serializable]
public enum GunStatPanelTypeEnum
{
    BaseDamage,
    ShotCooldownSeconds,
    Penetration,
    Knockback
}