using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public GameObject weaponPrefab;
    public AnimatorController weaponAnimatorController;
    public WeaponValues weaponValues;
}