using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public Rigidbody2D rb;

    public float minSpeed = 1;
    public float maxSpeed = 3f;

    float timer;

    public float enemyTravelDistance;

    float speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(2f, 6f);
    }

    
    void FixedUpdate()
    {
        Vector2 forward = new Vector2(transform.right.x, transform.right.y);
        rb.MovePosition(rb.position + forward * Time.fixedDeltaTime * speed * enemyTravelDistance);
    }

     void Update()
    {
        timer += Time.deltaTime;

        if (timer > 7)
        {
            Destroy(gameObject);
        }
    }
}
