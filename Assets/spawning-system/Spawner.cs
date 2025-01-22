using UnityEngine;
using System.Collections.Generic;

public class spawner : MonoBehaviour
{
    /**
    *   Spawner interface. The level system will call this function to spawn enemies on the set coordinates (spawnPoints).
    */
    public void Spawn() {
        for (int i = 0; i < count; ++i){
            int spawnPointIdx = Random.Range(0, spawnPoints.Count);
            Instantiate(enemyPrefab, spawnPoints[spawnPointIdx], Quaternion.identity);
        }
    }

    // This is for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (spawnPoints != null)
        {
            foreach (var spawnPoint in spawnPoints)
            {
                Gizmos.DrawSphere(spawnPoint, 0.5f);
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        count = Random.Range(minCount, maxCount);
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {

    }    

    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private int minCount;
    [SerializeField]
    private int maxCount;
    private int count;
    [SerializeField]
    private float difficultyMultiplier; // global multiplier to boost enemy stats based on difficulty level
    [SerializeField]
    private List<Vector2> spawnPoints;
}
