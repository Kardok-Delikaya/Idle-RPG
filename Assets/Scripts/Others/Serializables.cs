using System;

//Silah değerleri
[Serializable]
public class WeaponValues
{
    public float damage;
    public float cooldown;
    public float specialAttackCooldown;
    public float range;
    public float knockback;
    public float lifeSteal;
    public bool canDoSpecialAttack;
    public AttackType attackType;

    public WeaponValues(float damage, float cooldown, float specialAttackCooldown, float range, float knockback, float lifeSteal, bool canDoSpecialAttack, AttackType attackType)
    {
        this.damage = damage;
        this.cooldown = cooldown;
        this.specialAttackCooldown = specialAttackCooldown;
        this.range = range;
        this.knockback = knockback;
        this.lifeSteal = lifeSteal;
        this.canDoSpecialAttack = canDoSpecialAttack;
        this.attackType = attackType;
    }
    
    //Silah değerlerinin artması için çağırılması gereken  kod parçası
    internal void Upgrade(WeaponValues weaponValues)
    {
        damage += weaponValues.damage;
        cooldown += weaponValues.cooldown;
        specialAttackCooldown += weaponValues.specialAttackCooldown;
        range += weaponValues.range;
        knockback+=weaponValues.knockback;
        lifeSteal+=weaponValues.lifeSteal;
        canDoSpecialAttack = weaponValues.canDoSpecialAttack;
    }
}

//Oyuncu değerleri
[Serializable]
public class PlayerStats
{
    public float maxHealth;
    public float moveSpeed;
    public float coolDownMultiplier;

    public PlayerStats(float maxHealth, float moveSpeed, float coolDownMultiplier)
    {
        this.maxHealth = maxHealth;
        this.moveSpeed = moveSpeed;
        this.coolDownMultiplier = coolDownMultiplier;
    }

    //Karakter değerlerinin artması için çağırılması gereken kod parçası
    internal void Upgrade(PlayerStats playerStats)
    {
        maxHealth += playerStats.maxHealth;
        moveSpeed += playerStats.moveSpeed;
        coolDownMultiplier += playerStats.coolDownMultiplier;
    }
}