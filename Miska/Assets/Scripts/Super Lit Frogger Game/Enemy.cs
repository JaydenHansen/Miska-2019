using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemySpawner m_spawner;
    public Rigidbody2D rb;

    public float minSpeed = 1;
    public float maxSpeed = 3f;

    float timer;

    public float enemyTravelDistance;

    float speed = 1f;

    // Start is called before the first frame update
    /// <summary>
    /// Chooses between one of the speeds
    /// </summary>
    void Start()
    {
        speed = Random.Range(2f, 6f);
    }

    /// <summary>
    /// Moves the enemy in a direction at the set speed
    /// </summary>
    void FixedUpdate()
    {
        Vector2 forward = new Vector2(transform.right.x, transform.right.y);
        rb.MovePosition(rb.position + forward * Time.fixedDeltaTime * speed * enemyTravelDistance);
    }

    /// <summary>
    /// Destroys enemy after 7 seconds
    /// </summary>
     void Update()
    {
        timer += Time.deltaTime;

        if (timer > 7)
        {
            m_spawner.EnemyDeath();
            Destroy(gameObject);
        }
    }
}
