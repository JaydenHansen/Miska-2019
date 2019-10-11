using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public float spawnDelay = .6f;

    public GameObject Enemy;

    public Transform[] spawnPoints;

    float nextTimeToSpawn = 0f;
    // Update is called once per frame
    void Update()
    {
        if (nextTimeToSpawn <= Time.time)
        {
            SpawnEnemy();
            nextTimeToSpawn = Time.time + spawnDelay;
        }
    }

    void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        Instantiate(Enemy, spawnPoint.position, spawnPoint.rotation);
    }
}
