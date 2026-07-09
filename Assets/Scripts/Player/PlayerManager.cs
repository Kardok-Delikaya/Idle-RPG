using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    private CharacterController _controller;
    private Animator _anim;
    private Vector2 _inputVector;

    private readonly int _speedMash = Animator.StringToHash("Speed");
    private readonly float _rotationSpeed = 720f;

    [Header("Player Stats")] [SerializeField]
    private PlayerStats playerStats = new PlayerStats(100, 5, 1);

    private float _currentHealth;
    private bool _isDead;
    private bool _isInteracting;
    private float _normalAttackCoolDownTimer;
    private float _specialAttackCoolDownTimer;

    private int _currentExperience;
    private const int ExperienceNeededToLevelUp = 5;

    [Header("Weapon")] 
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private WeaponData currentWeaponData;
    [SerializeField] private LayerMask damageableLayer;
    private WeaponValues _equippedWeaponValues;
    private Vector3 _closestDamageable;

    [Header("Special Attack Objects")] [SerializeField]
    private SwordProjectile swordProjectile;

    //Oyunda birden fazla oyuncu objesi olmaması için kullanılan kod parçası
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    //Aktif edildiği zaman oyuncu objesi, GameEvents'teki belli olaylara abone olur. Bu sayede bazı olaylar olduğunda void'ler çalıştırır.
    void OnEnable()
    {
        GameEvents.OnEnemyKilled += GainExperience;
        GameEvents.OnUpgradeSelected += UpgradeWeaponAndPlayerStats;
        GameEvents.OnGetFullHpButtonPressed += GetFullHp;
        GameEvents.OnIdleAnimationStarted += IdleAnimationStarted;
    }

    //Deaktif edildiğinde abonelikten çıkar. Bu sayede tekrar aktif edilirse birden fazla kez çalışmaz void'ler.
    void OnDisable()
    {
        GameEvents.OnEnemyKilled -= GainExperience;
        GameEvents.OnUpgradeSelected -= UpgradeWeaponAndPlayerStats;
        GameEvents.OnGetFullHpButtonPressed -= GetFullHp;
        GameEvents.OnIdleAnimationStarted -= IdleAnimationStarted;
    }

    //Oyun başında silah ve karakter değerleri atanır, UI'da değerlerin gözükmesi için GameEvents'e haber gönderilir.
    void Start()
    {
        _anim = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();

        InitializeWeapon(currentWeaponData);

        _currentHealth = playerStats.maxHealth;
        GameEvents.PublishPlayerHealthChanged(_currentHealth, playerStats.maxHealth);
    }

    private void Update()
    {
        if (_isDead || _isInteracting) return;

        Combat();
        Movement();
    }

    #region Combat
    //Silah, karakterin sağ elindeki bir objede yaratılır ve değerleri oyuncudaki değere atanır, ayrıca animasyon ağacı da animator component'ine atanır.
    private void InitializeWeapon(WeaponData weaponData)
    {
        if (weaponData == null) return;

        Instantiate(weaponData.weaponPrefab, weaponHolder);

        _equippedWeaponValues = new WeaponValues(weaponData.weaponValues.damage, weaponData.weaponValues.cooldown,
            weaponData.weaponValues.specialAttackCooldown, weaponData.weaponValues.range,
            weaponData.weaponValues.knockback, weaponData.weaponValues.lifeSteal,
            weaponData.weaponValues.canDoSpecialAttack, weaponData.weaponValues.attackType);

        _anim.runtimeAnimatorController = weaponData.weaponAnimatorController;
    }

    //Oyuncuya en yakın düşman seçilmesi için kullanılan kod parçası
    private void GetClosestEnemy()
    {
        var damageableObjects =
            Physics.OverlapSphere(transform.position, _equippedWeaponValues.range, damageableLayer);
        var minDist = Mathf.Infinity;

        foreach (var damageable in damageableObjects)
        {
            if (damageable.GetComponent<Enemy>().IsDead)
            {
                _closestDamageable = Vector3.zero;
                return;
            }
            
            var dist = Vector3.Distance(damageable.transform.position, transform.position);

            if (dist < minDist)
            {
                _closestDamageable = damageable.transform.position;
                minDist = dist;
            }
        }

        if (damageableObjects.Length == 0)
            _closestDamageable = Vector3.zero;
    }

    //Bekleme süreleri sürekli azaltılır ve eğer ki normal saldırı sıfın altındaysa yakında düşman aranır. Eğer ki bulunursa silah ile saldırılır.
    private void Combat()
    {
        _normalAttackCoolDownTimer -= playerStats.coolDownMultiplier * Time.deltaTime;
        _specialAttackCoolDownTimer -= playerStats.coolDownMultiplier * Time.deltaTime;

        if (_normalAttackCoolDownTimer <= 0)
        {
            GetClosestEnemy();

            if (_closestDamageable != Vector3.zero)
            {
                AttackWithCurrentWeapon();
            }
        }
    }

    //En yakındaki düşmana döner karakter ve animasyon oynar. Eğer ki özel saldırı yapılıyorsa yapılır, yapılmazsa normal saldırı yapılır. Silahın türüne göre farklı saldırılar çağırılabilir.
    private void AttackWithCurrentWeapon()
    {
        _isInteracting = true;
        transform.LookAt(_closestDamageable);
        _normalAttackCoolDownTimer = _equippedWeaponValues.cooldown;

        if (_equippedWeaponValues.canDoSpecialAttack && _specialAttackCoolDownTimer < 0)
        {
            _specialAttackCoolDownTimer = _equippedWeaponValues.specialAttackCooldown;

            _anim.Play("Special_Attack", 0, 0f);

            return;
        }

        _anim.Play("Attack", 0, 0f);
    }

    //Kılıçta kullanılan düz saldırı. Oyuncunun etrafındaki tüm düşmanları bulur, ardından önünde olan düşmanları seçerek onlara hasar verir. Bu sayede baktığı yöndeki düşmanlar hasar alır sadece.
    public void SlashAttack()
    {
        Collider[] hitColliders =
            Physics.OverlapSphere(transform.position, _equippedWeaponValues.range, damageableLayer);

        Vector3 playerForward = transform.forward;
        Vector3 playerPos = transform.position;

        foreach (Collider col in hitColliders)
        {
            IDamageable damageable = col.GetComponent<IDamageable>();
            if (damageable == null) continue;

            Vector3 directionToEnemy = col.transform.position - playerPos;
            directionToEnemy.y = 0f;
            directionToEnemy.Normalize();

            float dotProduct = Vector3.Dot(playerForward, directionToEnemy);

            if (dotProduct > 0f)
            {
                damageable.TakeDamage(_equippedWeaponValues.damage, _equippedWeaponValues.knockback);

                GetDamage(-_equippedWeaponValues.damage * (_equippedWeaponValues.lifeSteal / 100));
            }
        }
    }

    //Kılıcın özel saldırısı için fırlatılabilir kılıç, verilen değerler atanarak aktif edilir.
    public void SpecialSlashAttack()
    {
        swordProjectile.InitializeProjectile(_equippedWeaponValues.damage * 2, _equippedWeaponValues.knockback,
            -_closestDamageable, transform.position);
    }

    //Hasar alma kodu. Ayrıca değer negatif seçilince can yenilemek içinde kullanılabilir
    public void GetDamage(float damage)
    {
        if (_isDead) return;

        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            _isDead = true;
            //die
        }

        if (_currentHealth > playerStats.maxHealth)
        {
            _currentHealth = playerStats.maxHealth;
        }

        GameEvents.PublishPlayerHealthChanged(_currentHealth, playerStats.maxHealth);
    }

    #endregion

    //Oyuncunun canını full'lemek için çağırılan kod parçası, ayrıca öldüyse tekrar canlandırır
    private void GetFullHp()
    {
        _currentHealth = playerStats.maxHealth;
        _isDead = false;
        GameEvents.PublishPlayerHealthChanged(_currentHealth, playerStats.maxHealth);
    }

    //Silahların ve ya oyuncunun değer yükseltilmesi için çağırılan kod parçası
    private void UpgradeWeaponAndPlayerStats(UpgradeData upgradeData)
    {
        switch (upgradeData.upgradeType)
        {
            case UpgradeType.PlayerUpgrade:
                playerStats.Upgrade(upgradeData.playerStats);
                GetDamage(-upgradeData.playerStats.maxHealth);
                break;
            case UpgradeType.WeaponUpgrade:
                _equippedWeaponValues.Upgrade(upgradeData.weaponValues);
                break;
        }
    }

    //Düşman öldürülürse oyuncunun +1 deneyim puanı verir. Eğer ki 5 veya fazla puanı alırsa level atlar
    private void GainExperience()
    {
        _currentExperience++;

        if (_currentExperience >= ExperienceNeededToLevelUp)
        {
            _currentExperience -= ExperienceNeededToLevelUp;
            GameEvents.PublishPlayerLevelUp();
        }

        GameEvents.PublishPlayerExperienceChanged(_currentExperience, ExperienceNeededToLevelUp);
    }

    private void IdleAnimationStarted()
    {
        _isInteracting = false;
    }
    
    #region Movement

    //Bu kod ile haraket eder ve ettiği yöne bakar
    private void Movement()
    {
        Vector3 targetDirection = new Vector3(_inputVector.x, 0f, _inputVector.y).normalized;
        Vector3 targetVelocity = targetDirection * playerStats.moveSpeed;

        _controller.Move(targetVelocity * Time.deltaTime);

        Vector3 currentPosition = transform.position;
        currentPosition.y = 0f;
        transform.position = currentPosition;

        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        _anim.SetFloat(_speedMash, _inputVector.sqrMagnitude);
    }

    //Yeni input sistemi ile haraket edilmesi için, kodun bağlı olduğu objedeki Player Input'a atılır.
    public void HandleMovement(InputAction.CallbackContext context)
    {
        _inputVector = context.ReadValue<Vector2>();
    }
    
    #endregion
}