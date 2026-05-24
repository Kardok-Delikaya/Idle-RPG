using UnityEngine;

//Bu kod ile düşmanların değerleri saklanıyor ve kolayca değiştirilmesi için veri tutucu diyebileceğimiz editörden değiştiribileceğimiz dosyalar oluşturuyor.

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
