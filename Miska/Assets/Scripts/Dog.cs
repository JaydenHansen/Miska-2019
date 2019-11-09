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
    public float m_runThreshold;
    public DogArea m_area;

    NavMeshAgent m_agent;
    Target m_currentTarget;
    float m_baseStoppingDistance;
    Animator m_animator;
    float m_pickupDelay;

    public AK.Wwise.Event m_barkSound;
    public AK.Wwise.Event m_barkStop;

    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_agent.updateRotation = false;
        m_agent.updateUpAxis = false;
        m_baseStoppingDistance = m_agent.stoppingDistance;

        m_animator = GetComponent<Animator>();
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
                        m_agent.stoppingDistance = 0.5f;
                        m_pickupDelay = 1;
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
                            m_agent.stoppingDistance = 0.5f;
                            m_pickupDelay = 1;
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

        if (m_agent.remainingDistance > m_runThreshold)
        {
            m_agent.speed = 4;
        }
        else
        {
            m_agent.speed = 3;
        }

        if (m_currentTarget == Target.Ball)
        {
            m_pickupDelay += Time.deltaTime;
            if ((transform.position - m_ball.transform.position).magnitude <= 1f && m_pickupDelay >= 1 && Mathf.Abs(m_ball.transform.position.y - transform.position.y) < 0.1)
            {
                m_animator.SetTrigger("Pickup");
                m_pickupDelay = 0;
            }
            if (m_agent.velocity.sqrMagnitude == 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 10, 1 << LayerMask.NameToLayer("Ground")))
                {
                    Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    Vector3 direction = m_ball.transform.position - transform.position;
                    direction.y = 0;
                    targetRotation *= Quaternion.LookRotation(direction.normalized);

                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10);
                }
            }
        }

        m_animator.SetFloat("Speed", m_agent.velocity.magnitude);

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

    public void Spawn()
    {
        gameObject.SetActive(true);
    }

    public void StartPat()
    {
        m_barkStop.Post(gameObject);
        m_animator.SetTrigger("Pat");
    }
}
