using UnityEngine;

//Yükseltme için obje oluşturulur, ayrıca eğer ki yükseltmenin devamı varsa bunun için de farklı yükseltme verileri atanabilir.
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
