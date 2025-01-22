using UnityEngine;
using System.Collections.Generic;

public class spawner : MonoBehaviour
{
    /**
    *   Spawner interface. The level system will call this function to spawn enemies on the set coordinates (spawnPoints).
    */
    public void Spawn() {
        foreach (Vector2 spawnPoint in spawnPoints) {
            Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
        }
    }

    private void SetTimeUntilSpawn() {
        timeUntilSpawn = Random.Range(minTime, maxTime);
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
        // needs to set timeUntilSpawn if we want to spawn multiple enemies
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        // needs to decrement timeUntilSpawn and call Spawn() when it reaches 0 if we want to spawn multiple enemies
    }    

    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private float minTime;
    [SerializeField]
    private float maxTime;
    private float timeUntilSpawn;
    [SerializeField]
    private float difficultyMultiplier; // global multiplier to boost enemy stats based on difficulty level
    [SerializeField]
    private List<Vector2> spawnPoints;
}
