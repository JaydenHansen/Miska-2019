using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum Target
{
    Player,
    Ball,
    Base
};

[RequireComponent(typeof(NavMeshAgent))]
public class Dog : MonoBehaviour
{
    public Transform m_player;
    public Ball m_ball;
    public Transform m_basePosition;
    public float m_radius;
    public float m_distanceBehindPlayer;
    public DogArea m_area;

    NavMeshAgent m_agent;
    Target m_currentTarget;
    float m_baseStoppingDistance;

    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_agent.updateRotation = false;
        m_baseStoppingDistance = m_agent.stoppingDistance;
    }

    
    void Update()
    {
        switch (m_currentTarget)
        {
            case Target.Player:
                {
                    if (m_ball.Parent == Parent.None && m_area.Contains(m_ball.transform.position))
                    {
                        m_currentTarget = Target.Ball;
                        m_agent.stoppingDistance = 0;
                        break;
                    }                    
                    if (!m_area.Contains(m_player.position))
                    {
                        m_currentTarget = Target.Base;
                        m_agent.stoppingDistance = 0;
                    }
                    break;
                }
            case Target.Ball:
                {
                    if (m_ball.Parent == Parent.Dog || m_ball.Parent == Parent.Player)
                    {
                        m_currentTarget = Target.Player;
                        m_agent.stoppingDistance = m_baseStoppingDistance;
                    }                    
                    if (!m_area.Contains(m_ball.transform.position))
                    {
                        if (m_area.Contains(m_player.position))
                        {
                            m_currentTarget = Target.Player;
                            m_agent.stoppingDistance = m_baseStoppingDistance;
                        }
                        else
                        {
                            m_currentTarget = Target.Base;
                            m_agent.stoppingDistance = 0;
                        }
                    }
                    break;
                }
            case Target.Base:
                {
                    if (m_ball.Parent == Parent.None)
                    {
                        if (m_area.Contains(m_ball.transform.position))
                        {
                            m_currentTarget = Target.Ball;
                            m_agent.stoppingDistance = 0;
                        }
                    }
                    else
                    {                        
                        if (m_area.Contains(m_player.position))
                        {
                            m_currentTarget = Target.Player;
                            m_agent.stoppingDistance = m_baseStoppingDistance;
                        }
                    }
                    break;
                }            
        }

        NavMeshHit hit;
        Vector3 targetPosition = Vector3.zero;
        switch (m_currentTarget)
        {
            case Target.Player:
                targetPosition = m_player.position;
                break;
            case Target.Ball:
                targetPosition = m_ball.transform.position;
                break;
            case Target.Base:
                targetPosition = m_basePosition.position;
                break;
        }

        m_agent.SetDestination(targetPosition);           
        //if (NavMesh.SamplePosition(targetPosition, out hit, 1, NavMesh.AllAreas))
        //{
        //    m_agent.SetDestination(hit.position);
        //}
        

        if (m_agent.velocity.sqrMagnitude != 0)
            transform.rotation = Quaternion.LookRotation(m_agent.velocity.normalized);
        else
            transform.forward = (new Vector3(m_player.position.x, 0, m_player.position.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
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
