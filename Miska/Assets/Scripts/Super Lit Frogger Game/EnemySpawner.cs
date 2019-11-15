using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public float spawnDelay = .6f;

    public GameObject Enemy;

    public Transform[] spawnPoints;

    float nextTimeToSpawn = 0f;

    LinkedList<GameObject> m_spawned;

    private void Start()
    {
        m_spawned = new LinkedList<GameObject>();
    }

    // Update is called once per frame
    /// <summary>
    /// Spawns enemy at the correct time
    /// </summary>
    void Update()
    {
        
        if (nextTimeToSpawn <= Time.time)
        {
            SpawnEnemy();
            nextTimeToSpawn = Time.time + spawnDelay;
        }
    }

    /// <summary>
    /// Spawns at one of the spawn points
    /// </summary>
    void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        GameObject enemy = Instantiate(Enemy, spawnPoint.position, spawnPoint.rotation, spawnPoint);
        m_spawned.AddLast(enemy);
        enemy.GetComponent<Enemy>().m_spawner = this;
    }

    /// <summary>
    /// Destroys all the enemies
    /// </summary>
    public void ResetGame()
    {
        foreach(GameObject enemy in m_spawned)
        {
            Destroy(enemy);
        }
        m_spawned.Clear();
    }

    /// <summary>
    /// Enemy Gets removed from list when it dies
    /// </summary>
    public void EnemyDeath()
    {
        m_spawned.RemoveFirst();
    }
}
