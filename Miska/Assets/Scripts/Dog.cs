using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Dog : MonoBehaviour
{
    public Transform m_player;
    public Transform m_basePosition;
    public float m_radius;
    public float m_distanceBehindPlayer;

    NavMeshAgent m_agent;
    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_agent.updateRotation = false;
    }

    
    void Update()
    {
        NavMeshHit hit;    
        if (NavMesh.SamplePosition(m_player.position, out hit, 1, NavMesh.AllAreas))
        {
            float distance = (m_player.position - m_basePosition.position).sqrMagnitude;
            if (distance <= m_radius * m_radius)
            {                
                m_agent.SetDestination(hit.position);
            }
            else
            {
                m_agent.SetDestination(m_basePosition.position);
            }
        }
        else
        {
            m_agent.SetDestination(m_basePosition.position);
        }

        if (m_agent.velocity.sqrMagnitude != 0)
            transform.rotation = Quaternion.LookRotation(m_agent.velocity.normalized);
        else
            transform.forward = (new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(m_player.position.x, 0, m_player.position.z)).normalized;
    }

    public void Spawn()
    {
        gameObject.SetActive(true);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(m_player.position - (m_player.forward * m_distanceBehindPlayer), out hit, 5, -1))
        {
            transform.position = hit.position;           
        }

    }
}
