using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Play a specific animation on the player
/// </summary>
public class QuicktimePlayAnimation : QuicktimeResponse
{
    public Player m_player;
    public string m_animationName;
    public bool m_disableMovement;

    public override void OnStart()
    {
        
    }

    /// <summary>
    /// Plays the animation
    /// </summary>
    public override void OnSuccess()
    {
        m_player.Animator.Play(m_animationName);

        if (m_disableMovement)
        {
            m_player.MovementState = MovementState.Disabled;
            m_player.CharacterController.enabled = false;
        }
    }

    /// <summary>
    /// re-enables the movement
    /// </summary>
    public override void OnFailure()
    {
        if (m_disableMovement)
        {
            m_player.MovementState = MovementState.Walking;
        }
    }
}
