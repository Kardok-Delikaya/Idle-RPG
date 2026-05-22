using UnityEngine;

[System.Serializable]
public struct EnemyStats
{
    public float moveSpeed;
    public float maxHealth;
    public float damage;
    public float attackCoolDown;

    public EnemyStats(float moveSpeed, float maxHealth, float damage, float attackCoolDown)
    {
        this.moveSpeed = moveSpeed;
        this.maxHealth = maxHealth;
        this.damage = damage;
        this.attackCoolDown = attackCoolDown;
    }
}

[CreateAssetMenu(menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Stats")] 
    public EnemyStats enemyStats=new EnemyStats(1,1,1,1);
}
