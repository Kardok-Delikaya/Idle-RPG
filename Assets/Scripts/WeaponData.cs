using System;
using UnityEditor.Animations;
using UnityEngine;

[Serializable]
public class WeaponValues
{
    public float damage;
    public float cooldown;
    public float range;
    public float knockback;
    public float lifeSteal;

    public WeaponValues(float damage, float cooldown, float range, float knockback, float lifeSteal)
    {
        this.damage = damage;
        this.cooldown = cooldown;
        this.range = range;
        this.knockback = knockback;
        this.lifeSteal = lifeSteal;
    }

    internal void Upgrade(WeaponValues weaponValues)
    {
        damage += weaponValues.damage;
        cooldown += weaponValues.cooldown;
        range += weaponValues.range;
        knockback+=weaponValues.knockback;
        lifeSteal+=weaponValues.lifeSteal;
    }
}

[CreateAssetMenu]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public GameObject weaponPrefab;
    public AnimatorController weaponAnimatorController;
    public WeaponValues weaponValues;
}