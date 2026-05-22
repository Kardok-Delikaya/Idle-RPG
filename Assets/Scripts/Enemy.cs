using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float damage, float knockback);
}

public class Enemy : MonoBehaviour, IDamageable
{
    private EnemyStats _stats;
    private float _health;
    private float _attackCoolDown;
    public bool IsDead { get;private set; }
    
    public float Damage => _stats.damage;
    
    private Animator _anim;
    private readonly int _speedMash = Animator.StringToHash("Speed");

    [Header("Enemy Data")]
    [SerializeField] private EnemyData enemyData;
    
    private void Awake()
    {
        _stats = enemyData.enemyStats;
        _anim=GetComponent<Animator>();
    }

    public void InitializeEnemy()
    {
        _health = _stats.maxHealth;
        IsDead = false;
    }

    public void HandleEnemyUpdate(Transform player)
    {
        _attackCoolDown-=Time.deltaTime;
        
        transform.LookAt(player.position);

        if (Vector3.Distance(transform.position, player.position) > 1f)
        {
            transform.Translate(Vector3.forward * (_stats.moveSpeed * Time.deltaTime));
            _anim.SetFloat(_speedMash, 1);
        }
        else
        {
            _anim.SetFloat(_speedMash, 0);

            if (_attackCoolDown < 0)
            {
                PlayerManager.Instance.GetDamage(_stats.damage);
                _attackCoolDown = _stats.attackCoolDown;
            }
        }
        
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    public void TakeDamage(float damage, float knockback)
    {
        if (IsDead) return;

        _health-= damage;
        
        if (_health <= 0)
            Die();
    }

    private void Die()
    {
        IsDead = true;
        gameObject.SetActive(false);
    }
}