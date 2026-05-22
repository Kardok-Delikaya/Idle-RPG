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
    private float maxHealth = 100;

    [SerializeField] private float attackRate = 1;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float coolDownMultiplier = 1;
    private float _currentHealth;
    private bool _isDead;
    private float _coolDownTimer;

    [Header("Weapon")] [SerializeField] private Transform weaponHolder;
    [SerializeField] private WeaponData currentWeapon;
    [SerializeField] private LayerMask damageableLayer;
    private Transform _closestDamageable;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        _anim = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();

        InitializeWeapon(currentWeapon);

        _currentHealth = maxHealth;
        GameEvents.PublishPlayerHealthChanged(_currentHealth, maxHealth);
    }

    private void Update()
    {
        if (_isDead) return;

        Attack();
        Movement();
    }

    private void Attack()
    {
        _coolDownTimer -= coolDownMultiplier * Time.deltaTime;

        if (_coolDownTimer <= 0)
        {
            GetClosestEnemy();

            if (_closestDamageable != null)
            {
                AttackWithCurrentWeapon();
            }
        }
    }

    private void InitializeWeapon(WeaponData weaponData)
    {
        if (weaponData == null) return;

        Instantiate(weaponData.weaponPrefab, weaponHolder);
        _anim.runtimeAnimatorController = weaponData.weaponAnimatorController;
    }

    private void GetClosestEnemy()
    {
        var damageableObjects =
            Physics.OverlapSphere(transform.position, currentWeapon.weaponValues.range, damageableLayer);
        var minDist = Mathf.Infinity;

        foreach (var damageable in damageableObjects)
        {
            var dist = Vector3.Distance(damageable.transform.position, transform.position);

            if (dist < minDist)
            {
                _closestDamageable = damageable.transform;
                minDist = dist;
            }
        }

        if (damageableObjects.Length == 0)
            _closestDamageable = null;
    }

    private void AttackWithCurrentWeapon()
    {
        transform.LookAt(_closestDamageable);
        _coolDownTimer = currentWeapon.weaponValues.cooldown;
        _anim.Play("Attack", 0, 0f);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, currentWeapon.weaponValues.range, damageableLayer);

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
                damageable.TakeDamage(currentWeapon.weaponValues.damage, currentWeapon.weaponValues.knockback);
                GetDamage(-currentWeapon.weaponValues.damage * (currentWeapon.weaponValues.lifeSteal / 100));
            }
        }
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

        GameEvents.PublishPlayerHealthChanged(_currentHealth, maxHealth);
    }

    #region Movement

    private void Movement()
    {
        Vector3 targetDirection = new Vector3(_inputVector.x, 0f, _inputVector.y).normalized;

        Vector3 targetVelocity = targetDirection * moveSpeed;

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