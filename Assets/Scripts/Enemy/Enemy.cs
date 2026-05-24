using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    private EnemyStats _stats;
    private float _health;
    private float _attackCoolDown;
    public bool IsDead { get;private set; }
    
    public float Damage => _stats.damage;
    
    private Animator _anim;
    private readonly int _speedMash = Animator.StringToHash("Speed");

    private float _knockBackTimer;
    private float _knockBackPower;
    
    [Header("Enemy Data")]
    [SerializeField] private EnemyData enemyData;
    
    private void Awake()
    {
        _stats = enemyData.enemyStats;
        _anim=GetComponent<Animator>();
    }

    //Bu kod parçası ile düşmanlar ölse bile kolayca tekrar çalıştırabiliyoruz. Bu sayede sürekli baştan yaratmaya gerek kalmıyor.
    public void InitializeEnemy()
    {
        _health = _stats.maxHealth;
        IsDead = false;
    }

    //Tüm düşmanların kendi Update'i olması yerine sadece çalışanların EnemyManager kodunun update'inde çağırılması daha iyi olduğu için bu kod yazıldı.
    public void HandleEnemyUpdate(Transform player)
    {
        //eğer ki düşman hasar aldı ve savrulucaksa bu kod ile geri gidiyor ve başka bir kod çalışmıyor.
        if (_knockBackTimer > 0)
        {
            _knockBackTimer -= Time.deltaTime;
            transform.Translate(-Vector3.forward * (_knockBackPower * (_stats.moveSpeed * Time.deltaTime)));
            return;
        }
        
        //Sürekli saldırı bekleme süresi azaltılıyor.
        _attackCoolDown-=Time.deltaTime;
        
        MoveToPlayerAndAttackIfInRange(player);
    }

    //Bu kısım ile oyuncuya doğru düşmanlar sürekli haraket ediyor ve uygun mesafeye geldiğinde saldırıyorlar
    private void MoveToPlayerAndAttackIfInRange(Transform player)
    {
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
        
        //Yüskekliği sıfırda tutarak yer çekimi, rigidbody gibi performans isteyen
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    //Hasar alma kodu, Interfaces kodundaki IDamagable'dan geliyor. Bu kodla hasar alıyor 
    public void TakeDamage(float damage, float knockback)
    {
        if (IsDead) return;

        _health-= damage;

        if (_health <= 0)
        {
            Die();
            return;
        }

        if (knockback > 0)
        {
            GetKnockBack(knockback);    
        }
    }

    //Eğer ki geri savrulucaksa, savrulma süresi boyunca savrulma gücü hızıyla geri savurmak için değer atanıyor.
    private void GetKnockBack(float knockback)
    {
        _knockBackTimer = .5f;
        _knockBackPower = knockback;
    }
    
    //Ölünce düşman yok edilmektense sadece kapatılıyor ve Game Events ile oyuncuya düşman öldü mesajı iletiliyor. Bu sayede PlayerManager tecrübe puanı alma void'ini çalıştıyor.
    private void Die()
    {
        IsDead = true;
        GameEvents.PublishEnemyKilled();
        gameObject.SetActive(false);
    }
}