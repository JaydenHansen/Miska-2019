using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_agent.updateRotation = false;
        m_agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_returnToWater)
        {
            m_agent.SetDestination(m_waterReturnPos.position);
            if ((transform.position - m_waterReturnPos.position).magnitude < 0.1)
            {
                this.enabled = false;
                m_agent.enabled = false;
                m_wanderDuck.enabled = true;
            }
        }
        else
        {
            if ((m_player.position - m_baseArea.position).magnitude < m_areaRadius)
                m_agent.SetDestination(m_player.position);
            else
                m_agent.SetDestination(m_baseArea.position);
        }

        if (m_agent.velocity.sqrMagnitude != 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 10, 1 << LayerMask.NameToLayer("Ground") | LayerMask.NameToLayer("Water")))
            {
                Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                targetRotation *= Quaternion.LookRotation(new Vector3(m_agent.velocity.x, 0, m_agent.velocity.z));

                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);
            }
        }
    }

    public void Spawn()
    {
        gameObject.SetActive(true);     
    }

    public void ReturnToWater()
    {
        m_returnToWater = true;
        m_agent.stoppingDistance = 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (m_baseArea)
            Gizmos.DrawWireSphere(m_baseArea.position, m_areaRadius);
    }
}
