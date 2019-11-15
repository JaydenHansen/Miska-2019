using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The ai behaviour of the randomly wandering duck
/// </summary>
public class Duck : MonoBehaviour
{
    public float m_forwardDist;
    public float m_circleRadius;
    public float m_speed;
    public float m_distanceFromEdge;
    public AK.Wwise.Event m_duckSound;

    Vector3 m_velocity;
    int m_areaMask;
    Animator m_animator;

    // Start is called before the first frame update
    void Start()
    {
        m_areaMask = 1 << NavMesh.GetAreaFromName("Duck");
        m_animator = GetComponent<Animator>();
        m_duckSound.Post(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Wander();
        FleeEdge();

        transform.position += m_velocity * Time.deltaTime * m_speed;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(m_velocity.normalized), Time.deltaTime);
        m_animator.SetFloat("Speed", m_velocity.magnitude);
    }

    /// <summary>
    /// Seeks towards and random position around the duck
    /// </summary>
    void Wander()
    {
        Vector3 circlePos = transform.position + (m_velocity.normalized * m_forwardDist);
        Vector2 circle = Random.insideUnitCircle.normalized * m_circleRadius;
        Vector3 displacement = circlePos + new Vector3(circle.x, 0, circle.y);

        m_velocity += ((displacement - transform.position).normalized - m_velocity) * Time.deltaTime;
    }

    /// <summary>
    /// Flees the closest edge of the duck to keep it from wandering out of the area
    /// </summary>
    void FleeEdge()
    {
        NavMeshHit hit;
        if (NavMesh.FindClosestEdge(transform.position, out hit, m_areaMask))
        {
            if (hit.distance < m_distanceFromEdge) // if the edge isn't too far away
            {
                m_velocity += ((new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(hit.position.x, 0, hit.position.z)).normalized - m_velocity) * Time.deltaTime; // flee from the edge
            }
        }
    }
}
