using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Duck : MonoBehaviour
{
    public float m_forwardDist;
    public float m_circleRadius;
    public float m_speed;
    public float m_distanceFromEdge;
    public AK.Wwise.Event m_duckSound;

    Vector3 m_velocity;
    int m_areaMask;

    // Start is called before the first frame update
    void Start()
    {
        m_areaMask = 1 << NavMesh.GetAreaFromName("Duck");
        m_duckSound.Post(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Wander();
        FleeEdge();

        transform.position += m_velocity * Time.deltaTime * m_speed;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(m_velocity.normalized), Time.deltaTime);
    }

    void Wander()
    {
        Vector3 circlePos = transform.position + (m_velocity.normalized * m_forwardDist);
        Vector2 circle = Random.insideUnitCircle.normalized * m_circleRadius;
        Vector3 displacement = circlePos + new Vector3(circle.x, 0, circle.y);

        m_velocity += ((displacement - transform.position).normalized - m_velocity) * Time.deltaTime;
    }

    void FleeEdge()
    {
        NavMeshHit hit;
        if (NavMesh.FindClosestEdge(transform.position, out hit, m_areaMask))
        {
            if (hit.distance < m_distanceFromEdge)
            {
                m_velocity += ((new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(hit.position.x, 0, hit.position.z)).normalized - m_velocity) * Time.deltaTime;
            }
        }
    }
}
