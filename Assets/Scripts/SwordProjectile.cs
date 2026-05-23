using System.Collections.Generic;
using UnityEngine;

public class SwordProjectile : MonoBehaviour
{
    [Header("Move and Rotation Speed")]
    [SerializeField] private float speed = 9f;
    [SerializeField] private float rotationSpeed = 360f;

    private float _damage;
    private float _knockback;
    private float _currentYRotation;
    private Vector3 _moveDirection;
    private readonly HashSet<IDamageable> _damagedTargets = new HashSet<IDamageable>();

    public void InitializeProjectile(float damage, float knockback, Vector3 targetDirection, Vector3 spawnTransform)
    {
        _damagedTargets.Clear();
        _damage = damage;
        _knockback = knockback;
        
        gameObject.SetActive(true);
        transform.position = spawnTransform;
        
        _moveDirection = targetDirection-transform.position;
        _moveDirection.Normalize();
        
        Invoke(nameof(TurnOffProjectile),3f);
    }

    private void TurnOffProjectile()
    {
        gameObject.SetActive(false);
    }
    
    void Update()
    {
        transform.Translate(_moveDirection * speed * Time.deltaTime, Space.World);
        
        Vector3 currentPos = transform.position;
        currentPos.y = 0.5f;
        transform.position = currentPos;

        _currentYRotation += rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(90f, _currentYRotation, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            if (_damagedTargets.Add(damageable))
            {
                damageable.TakeDamage(_damage,_knockback);
            }
        }
    }
}
