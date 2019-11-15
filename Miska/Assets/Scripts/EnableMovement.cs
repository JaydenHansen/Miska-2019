using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// animation script to re-enable the player's movement when an
/// </summary>
public class EnableMovement : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // enables the players movement
        animator.gameObject.GetComponent<Player>().MovementState = MovementState.Walking;
        animator.gameObject.GetComponent<Player>().CharacterController.enabled = true;
    }
}
