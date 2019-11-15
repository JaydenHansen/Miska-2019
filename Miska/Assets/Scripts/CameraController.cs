using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LookAtMode
{
    Continuous,
    Once
};

/// <summary>
/// The main camera controller
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Setup")]
    public GameObject m_parent;
    public Camera m_camera;
    [Header("Movement")]
    public float m_pitchUpperLimit;
    public float m_pitchLowerLimit;
    public float m_rotationSpeed;
    public float m_rotationFriction;        
    [Header("LookAt")]
    public Transform m_lookAt;
    public float m_baseLookAtStrength;
    public float m_lookAtDeadzone;
    public float m_minMovement;
    [Header("Misc")]
    public bool m_lockYaw;
    public Vector2 m_yawBounds;

    private float m_yaw = 0.0f;
    private float m_pitch = 0.0f;
    private float m_yawVelocity = 0.0f;
    private float m_pitchVelocity = 0.0f;
    private Vector3 m_baseOffset;
    private Vector3 m_offset;
    private Vector3 m_velocity;
    private LookAtMode m_lookAtMode;
    private bool m_disableControl;
    private float m_lookAtStrength;

    public float Yaw
    {
        get { return m_yaw; }
    }
    public float Pitch
    {
        get { return m_pitch; }
    }
    public Vector3 BaseOffset
    {
        get { return m_baseOffset; }
    }
    public Vector3 Offset
    {
        get { return m_offset; }
        set { m_offset = value; }
    }
    public LookAtMode LookMode
    {
        get { return m_lookAtMode; }
        set { m_lookAtMode = value; }
    }
    public bool DisableControl
    {
        get { return m_disableControl; }
        set { m_disableControl = value; }
    }
    public float LookAtStrength
    {
        get { return m_lookAtStrength; }
        set { m_lookAtStrength = value; }
    }


    public Ray DirectionRay
    {
        get
        {
            Ray output = new Ray();
            output.origin = m_camera.transform.position;
            output.direction = m_camera.transform.forward;
            return output;
        }
    }

    // Use this for initialization
    void Start ()
    {
        // the offset of the camera from the parent
        m_baseOffset = transform.position - m_parent.transform.position;
        m_offset = m_baseOffset;

        m_lookAtStrength = m_baseLookAtStrength;

        // get the current pitch and yaw from the camera's rotation
        m_yaw = transform.rotation.eulerAngles.y;
        m_pitch = transform.rotation.eulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;
    }
	
	/// <summary>
    /// The main camera control from the mouse and the look at functionality
    /// </summary>
	void Update ()
    {        
        transform.position = (m_parent.transform.position + m_offset); // moves the camera with it's parent, keeps it's initial offset

        if (!m_disableControl) // if the player has control of the camera
        {
            m_yawVelocity += Input.GetAxis("Mouse X") * m_rotationSpeed; // mouse x controls the yaw
        }
        m_yaw += m_yawVelocity * Time.deltaTime; // uses velocity to smooth the camera

        float yawDrag = m_yawVelocity * m_rotationFriction * Time.deltaTime; // drag

        if (Mathf.Abs(yawDrag) > Mathf.Abs(m_yawVelocity)) // stops the drag from overcompensating at lower framerates
            m_yawVelocity = 0;
        else
            m_yawVelocity -= yawDrag;


        if (m_lockYaw) // if the yaw is locked
        {
            m_yaw = Mathf.Clamp(m_yaw, m_yawBounds.x, m_yawBounds.y); // keep the yaw between the bounds
        }
        else
        { 
            // keeps the yaw value between [-180, 180]
            if (m_yaw > 180)
            {
                m_yaw -= 360;
            }
            if (m_yaw < -180)
            {
                m_yaw += 360;
            }
        }


        if (!m_disableControl) // if the player has control of the camera
        {
            m_pitchVelocity -= Input.GetAxis("Mouse Y") * m_rotationSpeed; // mouse y controls the pitch
        }
        m_pitch += m_pitchVelocity * Time.deltaTime;

        float pitchDrag = m_pitchVelocity * m_rotationFriction * Time.deltaTime; // drag

        if (Mathf.Abs(pitchDrag) > Mathf.Abs(m_pitchVelocity)) // stops the drag from overcompensating at lower framerates
            m_pitchVelocity = 0;
        else
            m_pitchVelocity -= pitchDrag;

        // Keeps the pitch value between [lowerLimit, upperLimit]
        m_pitch = Mathf.Clamp(m_pitch, m_pitchLowerLimit, m_pitchUpperLimit);

        // If the camera has a look at object
        if (m_lookAt)
        {
            Vector3 lookAtEuler = Quaternion.LookRotation((m_lookAt.position - transform.position).normalized).eulerAngles; // gets the angle of the camera to the lookat object
            // keeps the value between [-180, 180]
            if (lookAtEuler.x > 180)
                lookAtEuler.x -= 360;
            if (lookAtEuler.y > 180)
                lookAtEuler.y -= 360;

            bool appliedMovement = false; // if camera is in the deadzone

            if (m_yaw > lookAtEuler.y + m_lookAtDeadzone) // if the yaw is greater than the lookat yaw
            {
                float angle = Mathf.DeltaAngle(m_yaw, lookAtEuler.y + m_lookAtDeadzone);
                if (Mathf.Abs(angle) < m_minMovement)
                {
                    angle = angle < 0 ? -m_minMovement : m_minMovement;
                }
                m_yawVelocity += angle * m_lookAtStrength * Time.deltaTime; // rotate towards the look at rotation the speed is dependant on the difference between the current yaw and the desired yaw
                appliedMovement = true;
            }
            else if (m_yaw < lookAtEuler.y - m_lookAtDeadzone)
            {
                float angle = Mathf.DeltaAngle(m_yaw, lookAtEuler.y - m_lookAtDeadzone);
                if (Mathf.Abs(angle) < m_minMovement)
                {
                    angle = angle < 0 ? -m_minMovement : m_minMovement;
                }
                m_yawVelocity += angle * m_lookAtStrength * Time.deltaTime;
                appliedMovement = true;
            }

            if (m_pitch > lookAtEuler.x + m_lookAtDeadzone)
            {
                float angle = Mathf.DeltaAngle(m_pitch, lookAtEuler.x + m_lookAtDeadzone);
                if (Mathf.Abs(angle) < m_minMovement)
                {
                    angle = angle < 0 ? -m_minMovement : m_minMovement;
                }
                m_pitchVelocity += angle * m_lookAtStrength * Time.deltaTime;
                appliedMovement = true;
            }
            else if (m_pitch < lookAtEuler.x - m_lookAtDeadzone)
            {
                float angle = Mathf.DeltaAngle(m_pitch, lookAtEuler.x - m_lookAtDeadzone);
                if (Mathf.Abs(angle) < m_minMovement)
                {
                    angle = angle < 0 ? -m_minMovement : m_minMovement;
                }
                m_pitchVelocity += angle * m_lookAtStrength * Time.deltaTime;
                appliedMovement = true;
            }           

            // if the look mode is set to only work once and the camera is in the deadzone
            if (m_lookAtMode == LookAtMode.Once && !appliedMovement)
            {
                m_lookAt = null; // stop looking
            }
            
            transform.rotation = Quaternion.Euler(m_pitch, m_yaw, 0); // apply the rotation
        }
        else
        {
            transform.rotation = Quaternion.Euler(m_pitch, m_yaw, 0); // apply the rotation
        }
    }

    /// <summary>
    /// Sets the pitch and yaw from a euler rotation
    /// </summary>
    /// <param name="euler">The target euler rotation</param>
    public void SetRotation(Vector3 euler)
    {
        m_yaw = euler.y;
        m_pitch = euler.x;
    }

    /// <summary>
    /// Disables the player's control of the camera
    /// </summary>
    public void DisableMovement()
    {
        m_disableControl = true;
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;
    }
    /// <summary>
    /// Enables the player's control of the camera
    /// </summary>
    public void EnableMovement()
    {
        m_disableControl = false;
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
}
