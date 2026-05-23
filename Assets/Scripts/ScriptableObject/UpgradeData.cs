using UnityEngine;

[CreateAssetMenu]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public string upgradeDescription;
    public UpgradeType upgradeType;
    public PlayerStats playerStats;
    public WeaponValues weaponValues;
    public UpgradeData upgradeToAddToList;
}
