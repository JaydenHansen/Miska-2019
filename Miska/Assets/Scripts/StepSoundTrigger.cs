using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSoundTrigger : MonoBehaviour
{
    Animator m_animator;

    //public AK.Wwise.Event m_stepEvent;



    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    //Triggered by Animation Event, plays Wwise Event
    public void OnStepTrigger()
    {
      //  m_stepEvent.Post(gameObject);
    }

    //Sets Terrain Material (from TerrainReader) to player's position's material for use by Wwise to determine footstep sound
    public void SetTerrainMaterial(TerrainMaterial terMat)
    {
        if (terMat == TerrainMaterial.Dirt)
        {
         //   AkSoundEngine.SetSwitch("Locomotion_Material", "Dirt", gameObject);
        }
        else if (terMat == TerrainMaterial.Grass)
        {
          //  AkSoundEngine.SetSwitch("Locomotion_Material", "Grass", gameObject);
        }
        else if (terMat == TerrainMaterial.Rock)
        {
            //AkSoundEngine.SetSwitch("Locomotion_Material", "Wood", gameObject);
        }
        else
        {
            Debug.Log("Invalid Terrain Material");
        }


    }

    public void SetMovementState(MovementState moveState, float moveSpeed)
    {
        //Sets movement state for headbob
        m_animator.SetInteger("MovementState", (int)moveState);
        m_animator.SetFloat("MV_Speed", moveSpeed);


        //Conditional set the movement State in Wwise (determines which type of movement sound to use)
        if (moveState == MovementState.Walking)
        {
          //  AkSoundEngine.SetSwitch("Locomotion_Speed", "Walk", gameObject);
        }
        if (moveState == MovementState.Sprinting)
        {
           // AkSoundEngine.SetSwitch("Locomotion_Speed", "Sprint", gameObject);
        }

    }

}
