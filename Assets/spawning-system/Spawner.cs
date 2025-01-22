using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    /**
    *   Spawner interface. The level system will call this function to spawn enemies on the set coordinates (spawnPoints).
    */
    public void Spawn() {
        for (int i = 0; i < _count; ++i){
            int spawnPointIdx = Random.Range(0, _spawnPoints.Count);
            Instantiate(_enemyPrefab, _spawnPoints[spawnPointIdx], Quaternion.identity);
        }
    }

    // This is for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_spawnPoints != null)
        {
            foreach (var spawnPoint in _spawnPoints)
            {
                Gizmos.DrawSphere(spawnPoint, 0.5f);
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _count = Random.Range(_minCount, _maxCount);
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {

    }    

    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private int _minCount;
    [SerializeField]
    private int _maxCount;
    private int _count;
    [SerializeField]
    private float _difficultyMultiplier; // global multiplier to boost enemy stats based on difficulty level
    [SerializeField]
    private List<Vector2> _spawnPoints;
}
