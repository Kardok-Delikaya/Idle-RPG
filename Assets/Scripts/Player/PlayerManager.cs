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

    [Header("Player Stats")] 
    [SerializeField] private PlayerStats playerStats = new PlayerStats(100, 5, 1);

    private float _currentHealth;
    private bool _isDead;
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

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void OnEnable()
    {
        GameEvents.OnEnemyKilled += GainExperience;
        GameEvents.OnUpgradeSelected += UpgradeWeaponAndPlayerStats;
    }

    void OnDisable()
    {
        GameEvents.OnEnemyKilled -= GainExperience;
        GameEvents.OnUpgradeSelected -= UpgradeWeaponAndPlayerStats;
    }

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
        if (_isDead) return;

        Combat();
        Movement();
    }

    #region Combat
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

    private void GetClosestEnemy()
    {
        var damageableObjects =
            Physics.OverlapSphere(transform.position, _equippedWeaponValues.range, damageableLayer);
        var minDist = Mathf.Infinity;

        foreach (var damageable in damageableObjects)
        {
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

    private void AttackWithCurrentWeapon()
    {
        transform.LookAt(_closestDamageable);
        _normalAttackCoolDownTimer = _equippedWeaponValues.cooldown;

        _anim.Play("Attack", 0, 0f);

        if (_equippedWeaponValues.canDoSpecialAttack && _specialAttackCoolDownTimer < 0)
        {
            _specialAttackCoolDownTimer = _equippedWeaponValues.specialAttackCooldown;

            switch (_equippedWeaponValues.attackType)
            {
                case AttackType.Slash:
                    SpecialSlashAttack();
                    break;
                case AttackType.Projectile:
                    break;
            }

            return;
        }

        switch (_equippedWeaponValues.attackType)
        {
            case AttackType.Slash:
                SlashAttack();
                break;
            case AttackType.Projectile:
                break;
        }
    }

    private void SlashAttack()
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

    private void SpecialSlashAttack()
    {
        swordProjectile.InitializeProjectile(_equippedWeaponValues.damage * 2, _equippedWeaponValues.knockback,
            _closestDamageable, transform.position);
    }

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

        GameEvents.PublishPlayerHealthChanged(_currentHealth, playerStats.maxHealth);
    }
    
    #endregion
    
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

    #region Movement

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

    public void HandleMovement(InputAction.CallbackContext context)
    {
        _inputVector = context.ReadValue<Vector2>();
    }

    #endregion
}