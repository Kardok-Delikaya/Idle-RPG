using UnityEngine;

//Silah verilerinin tutulması için obje oluşturulmasını sağlayan kod parçası
[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public GameObject weaponPrefab;
    public RuntimeAnimatorController  weaponAnimatorController;
    public WeaponValues weaponValues;
}