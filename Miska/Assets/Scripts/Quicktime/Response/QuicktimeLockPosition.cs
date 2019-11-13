using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Locks the position of the player
/// </summary>
public class QuicktimeLockPosition : QuicktimeResponse
{
    public Player m_player;
    public Transform m_setPosition;
    public Animator m_headbob;

    Vector3 m_oldPos;

    /// <summary>
    /// Disables the movement in the player script and disables the character controller
    /// Moves the character to the set position
    /// </summary>
    public override void OnSuccess()
    {
        m_player.MovementState = MovementState.Disabled;
        m_headbob.SetBool("isMoving", false); //sets headbob to "Idle"
        m_player.CharacterController.enabled = false;
        m_oldPos = m_player.transform.position;
        m_player.transform.position = m_setPosition.position;
    }

    /// <summary>
    /// Re-enables the movement for the player and moves the player back to their old position
    /// </summary>
    public override void OnFailure()
    {
        m_player.MovementState = MovementState.Walking;
        m_player.transform.position = m_oldPos;
        m_player.CharacterController.enabled = true;
    }
}
