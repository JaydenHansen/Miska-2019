using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Determines walking terrain, and handles Wwise elements to play relevant sound
/// </summary>
public class StepSoundTrigger : MonoBehaviour
{
    private     Animator        m_animator;
    public      AK.Wwise.Event  m_stepEvent;



    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Triggered by Animation Event, plays step event
    /// </summary>
    public void OnStepTrigger()
    {
        m_stepEvent.Post(gameObject);
    }

    /// <summary>
    /// Sets Terrain Material in Wwise
    /// </summary>
    /// <param name="terMat">Terrain Material being set in Wwise</param>
    public void SetTerrainMaterial(TerrainMaterial terMat)
    {
        if (terMat == TerrainMaterial.Dirt)
        {
            AkSoundEngine.SetSwitch("Locomotion_Material", "Dirt", gameObject);
        }
        else if (terMat == TerrainMaterial.Grass)
        {
            AkSoundEngine.SetSwitch("Locomotion_Material", "Grass", gameObject);
        }
        else if (terMat == TerrainMaterial.Wood)
        {
            AkSoundEngine.SetSwitch("Locomotion_Material", "Wood", gameObject);
        }
        else
        {
            Debug.Log("Invalid Terrain Material");
        }


    }

    /// <summary>
    /// Sets the Movement Parameters for Animation and Wwise Audio
    /// </summary>
    /// <param name="moveState">Movement state (walk/sprint)</param>
    /// <param name="moveSpeed">Speed of movement</param>
    public void SetMovementState(MovementState moveState, float moveSpeed)
    {
        //Sets movement state for headbob
        m_animator.SetInteger("MovementState", (int)moveState);
        m_animator.SetFloat("MV_Speed", moveSpeed);


        //Conditional set the movement State in Wwise (determines which type of movement sound to use)
        if (moveState == MovementState.Walking)
        {
            AkSoundEngine.SetSwitch("Locomotion_Speed", "Walk", gameObject);
        }
        if (moveState == MovementState.Sprinting)
        {
            AkSoundEngine.SetSwitch("Locomotion_Speed", "Sprint", gameObject);
        }

    }

}
