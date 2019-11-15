using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// the target of the dog
/// </summary>
enum Target
{
    Player,
    Ball,
    Base
};

/// <summary>
/// The ai controller of the dog
/// </summary>
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

    /// <summary>
    /// Handles switching between target and pathing
    /// </summary>
    void Update()
    {
        // Switches between targets
        switch (m_currentTarget)
        {
            case Target.Player: // if the current target is the player
                {
                    if (m_ball.Parent == Parent.None && m_area.Contains(m_ball.transform.position)) // if the ball has been thrown and is in the area of the dog
                    {
                        // changes the target to the ball
                        m_currentTarget = Target.Ball;
                        m_agent.stoppingDistance = 0.5f;
                        m_pickupDelay = 1;
                        break;
                    }
                    if (!m_area.Contains(m_player.position)) // if the player isn't in the dog area
                    {
                        // changes the target to the base position
                        m_currentTarget = Target.Base; 
                        m_agent.stoppingDistance = 0;
                    }
                    break;
                }
            case Target.Ball: // if the current targer is the ball
                {
                    if (m_ball.Parent == Parent.Dog || m_ball.Parent == Parent.Player) // if the ball has been picked up by either the player or the dog
                    {
                        // change the target to the player
                        m_currentTarget = Target.Player;
                        m_agent.stoppingDistance = m_baseStoppingDistance;
                    }
                    if (!m_area.Contains(m_ball.transform.position)) // if the ball isn't in the area
                    {
                        if (m_area.Contains(m_player.position)) // if the player is in the area
                        {
                            // chages the target to the player
                            m_currentTarget = Target.Player;
                            m_agent.stoppingDistance = m_baseStoppingDistance;
                        }
                        else
                        {
                            // changes the target to the base position
                            m_currentTarget = Target.Base;
                            m_agent.stoppingDistance = 0;
                        }
                    }
                    break;
                }
            case Target.Base: // if the current target is the base position
                {
                    if (m_ball.Parent == Parent.None) // if the ball has been thrown
                    {
                        if (m_area.Contains(m_ball.transform.position)) // if the ball is in the dog area
                        {
                            // change the current target to the ball
                            m_currentTarget = Target.Ball;
                            m_agent.stoppingDistance = 0.5f;
                            m_pickupDelay = 1;
                        }
                    }
                    else
                    {
                        if (m_area.Contains(m_player.position)) // if the player is in the area
                        {
                            // change the target to the player
                            m_currentTarget = Target.Player;
                            m_agent.stoppingDistance = m_baseStoppingDistance;
                        }
                    }
                    break;
                }
        }

        Vector3 targetPosition = Vector3.zero;
        switch (m_currentTarget) // changes the target oosition based on the target
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

        m_agent.SetDestination(targetPosition); // update the destination

        if (m_agent.remainingDistance > m_runThreshold) // if the target is further away than the run threshold
        {
            m_agent.speed = 4; // change the agent to the running speed
        }
        else
        {
            m_agent.speed = 3; // change the agent to the walking speed
        }

        if (m_currentTarget == Target.Ball) // if the current target is the ball
        {
            m_pickupDelay += Time.deltaTime;
            if ((transform.position - m_ball.transform.position).magnitude <= 1f && m_pickupDelay >= 1 && Mathf.Abs(m_ball.transform.position.y - transform.position.y) < 0.1) // if the ball is close to the player and not in the air
            {
                m_animator.SetTrigger("Pickup"); // start the pick up anim
                m_pickupDelay = 0; // has a delay to not spam the animation
            }
            if (m_agent.velocity.sqrMagnitude == 0) // if the dog isn't moving
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 10, 1 << LayerMask.NameToLayer("Ground"))) // raycast to the ground
                {
                    Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                    // face in the direction of the ball
                    Vector3 direction = m_ball.transform.position - transform.position;
                    direction.y = 0;
                    targetRotation *= Quaternion.LookRotation(direction.normalized);

                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10); // lerp between the current rotation and the target rotation
                }
            }
        }

        m_animator.SetFloat("Speed", m_agent.velocity.magnitude); // set the speed parameter of the animator

        if (m_agent.velocity.sqrMagnitude != 0) // if the dog is moving
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 10, 1 << LayerMask.NameToLayer("Ground"))) // raycast to the ground to get the normal
            {
                Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal); // gets the rotation to the normal of the ground below the dog

                targetRotation *= Quaternion.LookRotation(new Vector3(m_agent.velocity.x, 0, m_agent.velocity.z)); // adds the look rotation of the velocity

                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10); // lerp between the current rotation and the target rotation 
            }
        }
    }

    /// <summary>
    /// Enables the dog gameobject
    /// </summary>
    public void Spawn()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Starts the pat animation
    /// </summary>
    public void StartPat()
    {
        m_barkStop.Post(gameObject);
        m_animator.SetTrigger("Pat");
    }
}
