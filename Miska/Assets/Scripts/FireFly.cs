using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Firefly ai behavior that wanders around an area while avoiding terrain
/// </summary>
public class FireFly : MonoBehaviour
{
    public float m_forwardDist;
    public float m_circleRadius;
    public float m_speed;
    public Transform m_player;
    public Transform m_camera;
    public Terrain m_terrain;
    public FireFlySpawner m_spawner;

    Vector3 m_velocity;


    // Start is called before the first frame update
    void Start()
    {
        m_velocity = Vector3.forward;
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = m_camera.forward; // billboard

        Wander();
        FleeTerrain();
        StayInArea();

        transform.position += m_velocity * Time.deltaTime * m_speed;
    }

    /// <summary>
    /// Seeks towards a random position around the firefly
    /// </summary>
    void Wander()
    {
        Vector3 circlePos = transform.position + (m_velocity.normalized * m_forwardDist);
        Vector3 displacement = circlePos + (Random.insideUnitSphere * m_circleRadius);
       
        m_velocity += ((displacement - transform.position) - m_velocity) * Time.deltaTime;
    }

    /// <summary>
    /// flees from the terrain to stay above ground
    /// </summary>
    void FleeTerrain()
    {
        float terrainHeight = m_terrain.SampleHeight(transform.position); // gets the height of the terrain at the fireflies current position

        Vector3 t2f = new Vector3(0, transform.position.y, 0) - new Vector3(0, terrainHeight, 0);

        if (t2f.sqrMagnitude < 1)
        {
            t2f.y = Mathf.Abs(t2f.y); // always flee upwards
            m_velocity += (t2f.normalized - m_velocity) * Time.deltaTime; // flee
        }        
        if (transform.position.y < terrainHeight) // if the firefly is below the terrain reset it's position
        {
            transform.position = new Vector3(transform.position.x, terrainHeight, transform.position.z);
        }
    }

    // seeks towards the centre of the area when outside
    void StayInArea()
    {
        Vector3 direction;
        float distance = m_spawner.DistanceFromSpawn(transform.position, out direction); // gets the distance from the centre of the area
        if (distance > m_spawner.m_radius)
        {
            m_velocity += (direction - m_velocity) * Time.deltaTime; // seek to centre
        }
    }

    /// <summary>
    /// debug draw of the velocity
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + m_velocity);
    }
}
