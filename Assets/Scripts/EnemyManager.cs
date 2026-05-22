using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    private float _spawnTimer;
    private int _enemiesThatHasBeenKilled;

    [Header("References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector2 spawnArea;
    
    [Header("Enemy Pool")] 
    [SerializeField] private List<Enemy> inactiveEnemyPool = new List<Enemy>();
    private readonly List<Enemy> _activeEnemyPool = new List<Enemy>();

    private void Awake()
    {
        foreach (var inActiveEnemies in GetComponentsInChildren<Enemy>())
        {
            inactiveEnemyPool.Add(inActiveEnemies);
            inActiveEnemies.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        MoveActiveEnemies();
        DeactivateDeadEnemies();
        
        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0)
        {
            StartCoroutine(SummonEnemies());
        }
    }

    private IEnumerator SummonEnemies()
    {
        for (; inactiveEnemyPool.Count > 0;)
        {
            inactiveEnemyPool[0].gameObject.SetActive(true);
            inactiveEnemyPool[0].transform.position = GenerateRandomPosition();
            inactiveEnemyPool[0].InitializeEnemy();
            _activeEnemyPool.Add(inactiveEnemyPool[0]);
            inactiveEnemyPool.RemoveAt(0);

            yield return new WaitForSeconds(1f);
        }
    }

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

    private void MoveActiveEnemies()
    {
        foreach (var enemy in _activeEnemyPool)
        {
            enemy.HandleEnemyUpdate(playerTransform);
        }
    }

    private void DeactivateDeadEnemies()
    {
        for (int i = 0; i < _activeEnemyPool.Count;)
        {
            if (_activeEnemyPool[i].IsDead)
            {
                inactiveEnemyPool.Add(_activeEnemyPool[i]);
                _activeEnemyPool.RemoveAt(i);
                _enemiesThatHasBeenKilled++;
                
                if (_activeEnemyPool.Count == 0)
                {
                    _spawnTimer = 5f;
                }
            }
            else
            {
                i++;
            }
        }
    }
}
