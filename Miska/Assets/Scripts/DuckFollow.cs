using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The ai of the POI duck that follows the player
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class DuckFollow : MonoBehaviour
{
    public Transform m_player;
    public float m_distanceBehindPlayer;
    public Transform m_baseArea;
    public float m_areaRadius;
    public Transform m_waterReturnPos;
    public Duck m_wanderDuck;

    NavMeshAgent m_agent;
    bool m_returnToWater;
    Animator m_animator;
    float m_returnTimer;

    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
        m_agent.updateRotation = false;
        m_agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_returnToWater) // if the duck should return to the water
        {
            if (m_returnTimer < 0.967) // waits for the animation to finsh
            {
                m_returnTimer += Time.deltaTime;
            }
            else
            {
                m_agent.SetDestination(m_waterReturnPos.position); // moves back into the water
                if ((transform.position - m_waterReturnPos.position).magnitude < 0.1) // if the duck has reached the water
                {
                    this.enabled = false; // disable the following duck
                    m_agent.enabled = false; 
                    m_wanderDuck.enabled = true; // enable the wandering duck
                }
            }
        }
        else
        {
            if ((m_player.position - m_baseArea.position).magnitude < m_areaRadius) // if the player is in the duck area
                m_agent.SetDestination(m_player.position); // move to the player
            else
                m_agent.SetDestination(m_baseArea.position); // move back to the base position
        }

        m_animator.SetFloat("Speed", m_agent.velocity.magnitude); // sets the speed parameter of the animator

        if (m_agent.velocity.sqrMagnitude != 0) // if the duck is moving
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 10, 1 << LayerMask.NameToLayer("Ground") | LayerMask.NameToLayer("Water")))// raycast to the ground to get the normal
            {
                Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal); // gets the rotation to the normal of the ground below the duck
                targetRotation *= Quaternion.LookRotation(new Vector3(m_agent.velocity.x, 0, m_agent.velocity.z));// adds the look rotation of the velocity

                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);// lerp between the current rotation and the target rotation 
            }
        }
    }

    /// <summary>
    /// enables the duck gameobject
    /// </summary>
    public void Spawn()
    {
        gameObject.SetActive(true);     
    }

    /// <summary>
    /// start the duck returning to the water
    /// </summary>
    public void ReturnToWater()
    {
        m_returnToWater = true;
        m_agent.stoppingDistance = 0;
    }

    /// <summary>
    /// debug draw of the duck's area
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (m_baseArea)
            Gizmos.DrawWireSphere(m_baseArea.position, m_areaRadius);
    }
}
