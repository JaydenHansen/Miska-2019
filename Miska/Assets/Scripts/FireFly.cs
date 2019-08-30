using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFly : MonoBehaviour
{
    public float m_forwardDist;
    public float m_circleRadius;
    public float m_speed;
    public Transform m_player;
    public Transform m_camera;
    public Terrain m_terrain;

    Vector3 m_velocity;

    // Start is called before the first frame update
    void Start()
    {
        m_velocity = Vector3.forward;
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = m_camera.forward;
        Wander();
        Flee();
        FleeTerrain();

        transform.position += m_velocity * Time.deltaTime * m_speed;
    }

    void Wander()
    {
        Vector3 circlePos = transform.position + (m_velocity.normalized * m_forwardDist);
        Vector3 displacement = circlePos + (Random.insideUnitSphere * m_circleRadius);
       
        m_velocity += ((displacement - transform.position) - m_velocity) * Time.deltaTime;
    }
    void Flee()
    {
        Vector3 p2f = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(m_player.position.x, 0, m_player.position.z);
        if (p2f.sqrMagnitude < 1)
        {
            m_velocity += (p2f.normalized - m_velocity) * Time.deltaTime;
        }        
    }
    void FleeTerrain()
    {
        Vector3 t2f = new Vector3(0, transform.position.y, 0) - new Vector3(0, m_terrain.SampleHeight(transform.position), 0);
        if (t2f.sqrMagnitude < 1)
        {
            m_velocity += (t2f.normalized - m_velocity) * Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + m_velocity);
    }
}
