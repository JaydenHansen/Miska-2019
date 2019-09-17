using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuicktimePlayAnimation : QuicktimeResponse
{
    public Player m_player;
    public string m_animationName;
    public bool m_disableMovement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnStart()
    {
        
    }

    public override void OnSuccess()
    {
        m_player.Animator.Play(m_animationName);

        if (m_disableMovement)
        {
            m_player.CharacterController.enabled = false;
        }
    }

    public override void OnFailure()
    {
        if (m_disableMovement)
        {
            m_player.MovementState = MovementState.Walking;
        }
    }
}
