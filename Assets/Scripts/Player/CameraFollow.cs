using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target settings")]
    [SerializeField] private Transform target;

    [Header("Position Settings")]
    [SerializeField] private Vector3 offset = new Vector3(2f, 10f, -6f);
    [SerializeField] private float smoothSpeed = 10f;

    private void Start()
    {
        //Eğer ki karakter editörde atanmadıysa atanması için kullanılan kod parçası
        if (target == null) target= GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    //Bu kod ile kamera oyuncudan belirli mesafede kalıyor ve LateUpdate'te olma sebebi oyuncunun haraket kodu ile aynı anda çalışmaması, bu sayede kameranın titremesi engelleniyor.
    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.fixedTime);

        transform.position = smoothedPosition;
    }
}