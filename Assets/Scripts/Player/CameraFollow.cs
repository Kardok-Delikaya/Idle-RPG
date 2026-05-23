using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target settings")]
    [SerializeField] private Transform target; // Takip edilecek karakter

    [Header("Position Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 7f, -10f); // Karakterle kamera arasındaki sabit mesafe
    [SerializeField] private float smoothSpeed = 10f; // Takip hızı (ne kadar yüksek, o kadar sert takip)

    private void Start()
    {
        if (target == null) target= GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Karakterin tüm hareket ve rotasyon hesaplamaları (Update/FixedUpdate) bittikten sonra çalışır.
    // Bu sayede kameranın titremesi (stuttering) önlenir.
    private void LateUpdate()
    {

        // 1. Kameranın gitmek istediği ideal pozisyonu hesapla
        Vector3 desiredPosition = target.position + offset;

        // 2. Mevcut pozisyondan ideal pozisyona pürüzsüz bir geçiş (Lerp) yap
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.fixedTime);

        // 3. Kameranın pozisyonunu güncelle
        transform.position = smoothedPosition;
    }
}