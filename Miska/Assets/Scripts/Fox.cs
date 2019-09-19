using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Fox : MonoBehaviour
{
    public Transform[] m_waypoints;

    NavMeshAgent m_agent;
    Animator m_animator;
    int m_currentWaypoint;

    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
        m_currentWaypoint = 0;
        if (m_waypoints.Length > 1)
            m_agent.autoBraking = false;
        m_agent.updateRotation = false;
        m_agent.updateUpAxis = false;
        m_agent.SetDestination(m_waypoints[m_currentWaypoint].position);

    }

    // Update is called once per frame
    void Update()
    {
        m_animator.SetFloat("Speed", m_agent.velocity.magnitude);
        if (m_agent.remainingDistance < 0.1f)
        {
            if (m_currentWaypoint + 1 < m_waypoints.Length)
            {
                m_currentWaypoint++;
                m_agent.SetDestination(m_waypoints[m_currentWaypoint].position);
                if (m_currentWaypoint == m_waypoints.Length - 1)
                {
                    m_agent.autoBraking = true;
                }
            }
        }

        if (m_agent.velocity.sqrMagnitude != 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 10, 1 << LayerMask.NameToLayer("Ground")))
            {
                Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                targetRotation *= Quaternion.LookRotation(new Vector3(m_agent.velocity.x, 0, m_agent.velocity.z));

                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);
            }
        }
    }

    private void FixedUpdate()
    {
       
    }

    //private void OnDrawGizmosSelected()
    //{
    //    for(int i = 0; i < m_agent.path.corners.Length; i++)
    //    {
    //        Gizmos.DrawSphere(m_agent.path.corners[i], 0.5f);
    //        if (i != 0)
    //            Gizmos.DrawLine(m_agent.path.corners[i - 1], m_agent.path.corners[i]);
    //    }
    //}
}
