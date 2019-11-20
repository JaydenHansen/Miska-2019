using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The movement state of the player
/// </summary>
public enum MovementState
{
    Walking,
    Sprinting,
    Disabled
}

/// <summary>
/// The main player movement contoller
/// </summary>
public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float m_acceleration;
    public float m_sprintAcceleration;
    public float m_friction;
    public Vector3 m_gravity;
    [Header("")]
    public CameraController m_cameraController = null;
    public Transform m_pickup;

    private CharacterController m_characterController;
    private Animator m_animator;
    private Vector3 m_velocity;
    private bool m_grounded = false;
    private MovementState m_movementState = MovementState.Walking;
    private MovementState m_prevState;
    private bool m_hasPickup;
    private Pickup m_pickupObject;

    //[SerializeField]
    //private int m_trashCount;

    public StepSoundTrigger m_stepper;

    #region Getter/Setter
    public CharacterController CharacterController
    {
        get { return m_characterController; }
    }
    public Animator Animator
    {
        get { return m_animator; }
    }
    public MovementState MovementState
    {
        get { return m_movementState; }
        set { m_movementState = value; }
    }
    //public int TrashCount
    //{
    //    get { return m_trashCount; }
    //    set { m_trashCount = value; }
    //}
    #endregion

    // Use this for initialization
    void Start()
    {
        m_characterController = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_movementState != MovementState.Disabled) // if the player's movement is not disabled
        {          
            if (Input.GetKeyDown(KeyCode.LeftShift) && m_movementState == MovementState.Walking) // if the player is walking pressing left shift will start sprinting
            {
                m_movementState = MovementState.Sprinting;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift) && m_movementState == MovementState.Sprinting) // if the player is sprinting releasing left shift will stop sprinting
            {
                m_movementState = MovementState.Walking;
            }

            // Gets the movement relative to the direction the camera is facing
            Vector3 movementVector = Quaternion.Euler(0, m_cameraController.Yaw, 0) * Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")), 1);

            float acceleration = 0;
            switch (m_movementState) // changes the acceleration depending on the MovementState
            {
                case MovementState.Walking:
                    acceleration = m_acceleration;
                    break;
                case MovementState.Sprinting:
                    acceleration = m_sprintAcceleration;
                    break;
            }

            //m_velocity += m_gravity * Time.deltaTime;                      
            m_velocity += movementVector * acceleration * Time.deltaTime;

            if(m_stepper != null)
            {
                m_stepper.SetMovementState(m_movementState, movementVector.magnitude);
            }                      

            CollisionFlags collision = m_characterController.Move((m_velocity * Time.deltaTime) + (m_gravity * Time.deltaTime)); // moves using the character controller

            m_velocity -= new Vector3(m_velocity.x, 0, m_velocity.z) * m_friction * Time.deltaTime; // Drag only on the x/z

            transform.rotation = Quaternion.Euler(0, m_cameraController.Yaw, 0); // rotate the player in the direction of the camera
        }
        
        // Pickup
        if (Input.GetMouseButtonDown(0))
        {
            if (!m_hasPickup) // if the player doens't have a pickup
            {
                RaycastHit hit;
                if (Physics.Raycast(m_cameraController.DirectionRay, out hit)) // raycast out from the camera
                {
                    Pickup rock = hit.collider.GetComponent<Pickup>(); // if a pickup is hit
                    if (rock && !rock.Rigidbody.isKinematic)
                    {
                        // start the pickup
                        rock.StartPickup(m_pickup); 
                        Physics.IgnoreCollision(m_characterController, rock.Collider, true);
                        m_hasPickup = true;
                        m_pickupObject = rock;
                    }
                }
            }
            else // if the player already has a pickup 
            {                    
                if (m_pickupObject)
                {
                    m_pickupObject.EndPickup(); // drop the pickup
                    Physics.IgnoreCollision(m_characterController, m_pickupObject.GetComponent<Collider>(), false);
                }                

                m_hasPickup = false;
            }
        }
    }


    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    // THIS MAKES THE PLAYER SLIDE DOWN SLOPES... AND ALSO UP...
    //    //Vector3 velocityProjected = Vector3.Project(m_velocity, -hit.normal);
    //    //m_velocity -= velocityProjected;
    //}
}
