using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Duck : MonoBehaviour
{
    public float m_baseDelay;
    public float m_radius;

    NavMeshAgent m_agent;
    float m_timer;

    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer >= m_baseDelay)
        {
            Vector3 direction = Random.insideUnitSphere * m_radius;
            direction += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(direction, out hit, m_radius, -1))
            {
                m_agent.SetDestination(hit.position);
            }

            m_timer = 0;
        }
    }

    
}
