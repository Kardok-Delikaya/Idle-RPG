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

    //Oyuncunun upgrade aldığında değerlerin, fırlatılacağı yerin güncellenmesi, belli konuma ışınlanığ aktif edilmesini ve 3 saniye sonra kapanmasını sağlayan kod parçası
    public void InitializeProjectile(float damage, float knockback, Vector3 targetDirection, Vector3 spawnTransform)
    {
        _damagedTargets.Clear();
        _damage = damage;
        _knockback = knockback;
        
        _moveDirection = targetDirection-transform.position;
        _moveDirection.Normalize();
        
        transform.position = spawnTransform;
        gameObject.SetActive(true);
        
        Invoke(nameof(TurnOffProjectile),3f);
    }

    //Aktif edildiğinden bir süre sonra kapanması için çağırılan kod parçası
    private void TurnOffProjectile()
    {
        gameObject.SetActive(false);
    }
    
    //Update'te sürekli oyuncunun fırlattığı yönde ilerliyor ve kendi etrafında dönüyor
    void Update()
    {
        transform.Translate(_moveDirection * speed * Time.deltaTime, Space.World);
        
        Vector3 currentPos = transform.position;
        currentPos.y = 0.5f;
        transform.position = currentPos;

        _currentYRotation += rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(90f, _currentYRotation, 0f);
    }

    //Eğer ki IDamageable interface'ine sahip objeler ile çarpışırsa hasar vermesini sağlayan kod parçası. Ayrıca vurduklarını listeye ekleyerek aynı objeye birden fazla hasar verilmemesi sağlanıyor
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
