using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The ai behaviour of the fox
/// paths between a set of waypoints
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Fox : MonoBehaviour
{
    public Transform[] m_waypoints;

    NavMeshAgent m_agent;
    Animator m_animator;
    int m_currentWaypoint;
    public AK.Wwise.Event m_stepSound;

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
        m_animator.SetFloat("Speed", m_agent.velocity.magnitude); // sets the speed parameter of the animator

        if (m_agent.remainingDistance < 0.1f) // if the agent is near the current waypoint
        {
            if (m_currentWaypoint + 1 < m_waypoints.Length) // if there is another waypoint
            {
                m_currentWaypoint++; // go to the next waypoint
                m_agent.SetDestination(m_waypoints[m_currentWaypoint].position);

                if (m_currentWaypoint == m_waypoints.Length - 1) // if this is the last waypoint
                {
                    m_agent.autoBraking = true; 
                }
            }
        }

        if (m_agent.velocity.sqrMagnitude != 0) 
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 10, 1 << LayerMask.NameToLayer("Ground")))// raycast to the ground to get the normal
            {
                Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);// gets the rotation to the normal of the ground below the fox
                targetRotation *= Quaternion.LookRotation(new Vector3(m_agent.velocity.x, 0, m_agent.velocity.z));// adds the look rotation of the velocity

                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);// lerp between the current rotation and the target rotation 
            }
        }
    }

    /// <summary>
    /// Play's the step sound of the fox
    /// </summary>
    public void PlayStepSound()
    {
        m_stepSound.Post(gameObject);
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
