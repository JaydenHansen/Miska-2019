using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCharacterController : MonoBehaviour
{
    public float m_acceleration;
    public float m_crouchedAcceleration;
    public float m_friction;
    public float m_jumpSpeed;
    public int m_jumpAmount;
    public Vector3 m_gravity;
    public float m_wallClimbLength;
    public float m_wallClimbSpeed;
    public float m_wallRunLength;
    public float m_crouchLength;
    public CameraController m_cameraController = null;
    public LineRenderer m_grappleHook;

    public CapsuleCollider m_collider = null;
    private Vector3 m_velocity;
    public bool m_grounded = false;
    private bool m_crouching = false;
    private float m_colliderHeight;
    private float m_colliderCentre;
    private int m_jumps;
    private bool m_canWallClimb;
    private float m_wallClimbTimer;
    private float m_wallRunTimer;
    private bool m_canWallRun;
    private float m_crouchTimer;
    private Vector3 m_grappleLocation;
    private bool m_grappling;
    private float m_grappleLength;
    private bool m_onLedge;
    private Vector3 m_ledgeNormal;
	// Use this for initialization
	void Start ()
    {
        m_collider = GetComponent<CapsuleCollider>();

        m_colliderHeight = m_collider.height;
        m_colliderCentre = m_collider.center.y;

        Cursor.lockState = CursorLockMode.Locked;

        m_jumps = m_jumpAmount;
        m_wallClimbTimer = m_wallClimbLength;
        m_wallRunTimer = m_wallRunLength;
        m_crouchTimer = m_crouchLength;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //m_yaw += Input.GetAxis("Mouse X") * 2;

        if ((m_grounded || m_jumps > 0) && Input.GetKeyDown(KeyCode.Space))
        {
            m_velocity += new Vector3(0, m_jumpSpeed, 0);
            m_jumps--;
        }

        if (Input.GetKey(KeyCode.Space) && Physics.Raycast(transform.position, transform.forward, 1f) && (m_canWallClimb || m_wallClimbTimer > 0))
        {
            if (m_canWallClimb)
            {
                m_wallClimbTimer = m_wallClimbLength;
            }
            m_canWallClimb = false;
            m_velocity.y = m_wallClimbSpeed;
            m_wallClimbTimer -= Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Space) && (Physics.Raycast(transform.position, transform.right, 1f) || Physics.Raycast(transform.position, -transform.right, 1f)) && (m_canWallRun || m_wallRunTimer > 0))
        {
            if (m_canWallRun)
            {
                m_wallRunTimer = m_wallRunLength;
            }
            m_canWallRun = false;
            if (m_wallRunLength != 0)
                m_velocity.y = Mathf.Lerp(-5, 5, m_wallRunTimer / m_wallRunLength);
            m_wallRunTimer -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(transform.position, -transform.forward, 1f))
        {
            Vector3 direction = transform.forward;
            direction.y = 0.5f;
            m_velocity += direction.normalized * m_jumpSpeed / 2f;
        }

        if (!m_onLedge)
        {
            m_velocity += m_gravity * Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycast;
            if (Physics.Raycast(m_cameraController.DirectionRay, out raycast, 100))
            {
                m_grappling = true;
                m_grappleLocation = raycast.point;
                m_grappleLength = raycast.distance;
                m_grappleHook.enabled = true;
                m_grappleHook.SetPosition(0, transform.position);
                m_grappleHook.SetPosition(1, m_grappleLocation);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            m_grappling = false;
            m_grappleHook.enabled = false;
        }
        if (m_grappling)
        {
            m_grappleHook.SetPosition(0, transform.position);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (m_crouching)
            {
                RaycastHit[] raycast = Physics.RaycastAll(transform.position, Vector3.up, 2.0f);
                if ((raycast.Length > 0 && raycast[0].distance >= 2) || raycast.Length == 0)
                {
                    m_crouching = false;
                }
            }
            else
            {
                m_crouching = true;
            }    
        }
        if (m_crouching)
            m_crouchTimer -= Time.deltaTime;
        else
            m_crouchTimer += Time.deltaTime;
        if (m_crouchTimer > m_crouchLength)
            m_crouchTimer = m_crouchLength;
        if (m_crouchTimer < 0)
            m_crouchTimer = 0;

        m_collider.height = Mathf.Lerp(m_colliderHeight / 2, m_colliderHeight, m_crouchTimer / m_crouchLength);
        m_collider.center = new Vector3(0, Mathf.Lerp(m_colliderCentre / 2, m_colliderCentre, m_crouchTimer / m_crouchLength), 0);

        if (!m_onLedge)
        {
            Vector3 movementVector = Quaternion.Euler(0, m_cameraController.Yaw, 0) * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            if (m_grounded)
                m_velocity += movementVector * (m_crouching ? m_crouchedAcceleration : m_acceleration) * Time.deltaTime;
            else
                m_velocity += movementVector * m_acceleration * 0.1f * Time.deltaTime;

            if (m_grounded)
                m_velocity -= new Vector3(m_velocity.x, 0, m_velocity.z) * m_friction * Time.deltaTime;
        }
        else
        {
            Vector3 movementVector = m_ledgeNormal * Input.GetAxis("Horizontal");
            m_velocity += movementVector * m_acceleration * Time.deltaTime;

            m_velocity -= new Vector3(m_velocity.x, 0, m_velocity.z) * m_friction * Time.deltaTime;
        }

        transform.position += m_velocity * Time.deltaTime;

        transform.rotation = Quaternion.Euler(0, m_cameraController.Yaw, 0);

        //if (movementVector.magnitude == 0 && m_velocity.magnitude <= 0.1)
        //{
        //    m_velocity = Vector3.zero;
        //}
    }

    private void LateUpdate()
    {        
        Collider[] collisions = Physics.OverlapCapsule(m_collider.center + new Vector3(0, m_collider.height / 2, 0) + transform.position, m_collider.center - new Vector3(0, m_collider.height / 2, 0) + transform.position, m_collider.radius, -1, QueryTriggerInteraction.Collide);

        foreach (Collider c in collisions)
        {
            if (c == m_collider)
                continue;
            if (c.isTrigger)
            {
                m_onLedge = true;
                m_ledgeNormal = new Vector3(0, 0, 1);
                m_velocity.y = 0;
                continue;
            }

            Vector3 direction;
            float distance;
            if (Physics.ComputePenetration(m_collider, transform.position, Quaternion.Euler(0,0,0), c, c.transform.position, c.transform.rotation, out direction, out distance))
            {
                Vector3 penetration = direction * distance;
                Vector3 velocityProjected = Vector3.Project(m_velocity, -direction);

                transform.position += penetration;
                m_velocity -= velocityProjected;
            }
            //if (Vector3.Dot(direction, Vector3.up) > 0)
            //{
            //    colliderBelow = true;
            //    m_jumps = m_jumpAmount;
            //    m_canWallClimb = true;
            //}
        }

        if (m_grappling)
        {
            Vector3 grapple = m_grappleLocation - m_cameraController.DirectionRay.origin;
            float magnitude = grapple.magnitude;
            if (magnitude > m_grappleLength)
            {
                transform.position += grapple.normalized * (magnitude - m_grappleLength);
                Vector3 velocityProjected = Vector3.Project(m_velocity, -grapple.normalized);
                m_velocity -= velocityProjected;
            }
        }

        //m_grounded = colliderBelow;
        Vector3 origin = transform.position;
        origin.y += 0.1f;
        if (Physics.Raycast(origin, Vector3.down, 0.2f))
        {
            m_grounded = true;
            m_canWallClimb = true;
            m_canWallRun = true;
            m_jumps = m_jumpAmount;
        }
        else
        {
            m_grounded = false;
        }
    }
}
