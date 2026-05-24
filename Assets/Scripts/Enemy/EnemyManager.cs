using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector2 spawnArea;
    private float _spawnTimer;
    
    private readonly List<Enemy> _inactiveEnemyPool = new List<Enemy>();
    private readonly List<Enemy> _activeEnemyPool = new List<Enemy>();

    //Oyun ilk çalıştırıldığında EnemyManager oyun objesinin altındaki tüm düşman koduna sahip düşmanları aktif olmayan düşman listesine ekleyip kapatıyor. Bu sayede editörden listeyi değiştirmeye gerek kalmıyor.
    private void Awake()
    {
        foreach (var inActiveEnemies in GetComponentsInChildren<Enemy>())
        {
            _inactiveEnemyPool.Add(inActiveEnemies);
            inActiveEnemies.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        HandleActiveEnemiesUpdates();
        DeactivateDeadEnemies();

        //Eğer ki aktif düşman yoksa tekrar onları aktif etmeden önce 5 saniye bekleyip aktif etmeye başlıyor.
        if (_activeEnemyPool.Count==0)
        {
            _spawnTimer-= Time.deltaTime;

            if (_spawnTimer <= 0)
            {
                StartCoroutine(SummonEnemies());
            }
        }
    }

    //Aktif olmayan düşmanları listenin başından başlayıp tek tek kaldırıyor. i değerini aktif olmayan listeye eşitleyerek aktifleştirme sırasında düşmanlar oyuncu tarafından öldürülürse aktifleştirilmenin bozulmaması sağlanıyor.
    private IEnumerator SummonEnemies()
    {
        for (int i = _inactiveEnemyPool.Count; i > 0; i--)
        {
            _inactiveEnemyPool[0].transform.position = GenerateRandomPosition();
            _inactiveEnemyPool[0].InitializeEnemy();
            _inactiveEnemyPool[0].gameObject.SetActive(true);
            _activeEnemyPool.Add(_inactiveEnemyPool[0]);
            _inactiveEnemyPool.RemoveAt(0);

            yield return new WaitForSeconds(.1f);
        }

        _spawnTimer = 5f;
    }

    //Düşmanların aktifleştirildiği rastgele konumu döndüren kod parçası
    private Vector3 GenerateRandomPosition()
    {
        var position = new Vector3();

        var f = Random.value > .5f ? -1f : 1f;
        if (Random.value > .5f)
        {
            position.x = Random.Range(-spawnArea.x, spawnArea.x);
            position.z = spawnArea.y * f;
        }
        else
        {
            position.z = Random.Range(-spawnArea.y, spawnArea.y);
            position.x = spawnArea.x * f;
        }
        
        position.y = 0;
        
        return position;
    }

    //Aktif düşmanlardaki HandleEnemyUpdate voidini çağıran void
    private void HandleActiveEnemiesUpdates()
    {
        foreach (var enemy in _activeEnemyPool)
        {
            enemy.HandleEnemyUpdate(playerTransform);
        }
    }

    //Eğer ki düşman öldüyse onları kapatıp aktif listeden deaktif listeye yönlendiren kod parçası
    private void DeactivateDeadEnemies()
    {
        for (int i = 0; i < _activeEnemyPool.Count;)
        {
            if (_activeEnemyPool[i].IsDead)
            {
                _inactiveEnemyPool.Add(_activeEnemyPool[i]);
                _activeEnemyPool.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }
}
