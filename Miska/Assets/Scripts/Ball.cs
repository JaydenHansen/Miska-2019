using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Parent
{
    Player,
    Dog,
    None
}

/// <summary>
/// Handles the picking up and throwing of the dog's ball
/// </summary>
public class Ball : MonoBehaviour
{
    public float m_throwStrength;
    public Transform m_cameraArm;
    public Collider m_collider;
    public Collider m_qtCollider;
    public DogArea m_area;
    public Transform m_mouthTransform;
    public InteractWheel m_interactWheel;

    bool m_inHand = false;
    Rigidbody m_rigidbody;
    QuicktimeBase m_qtBase;
    bool m_onThisFrame;
    Parent m_parent;

    public AK.Wwise.Event m_ballCollideSound;


    public bool InHand
    {
        get { return m_inHand; }
    }
    public Parent Parent
    {
        get { return m_parent; }
    }
    public Rigidbody RigidBody
    {
        get { return m_rigidbody; }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_qtBase = GetComponent<QuicktimeBase>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_parent == Parent.None) // if the ball has been thrown
        {
            if (!m_area.Contains(transform.position)) // if the ball is out of the dog area
            {
                DogPickup(m_mouthTransform); // Resets the ball into the dogs mouth
            }
        }        

        if (m_inHand && !m_onThisFrame && m_parent == Parent.Player && Input.GetKeyDown(KeyCode.Mouse0)) // if the ball is in the player's hand and the player left clicks
        {
            // unparents the ball from the player
            m_inHand = false;
            transform.parent = null;
            m_parent = Parent.None;

            // enables the rigidbody and applies force in the direction of the camera
            m_rigidbody.isKinematic = false;
            m_rigidbody.AddForce(m_cameraArm.forward * m_throwStrength);

            // enables the quicktime so the player can pick it back up
            m_qtBase.enabled = true;
            m_qtCollider.enabled = true;
            m_collider.enabled = true;
        }

        m_onThisFrame = false;
    }

    /// <summary>
    /// places the ball into the players hand
    /// </summary>
    /// <param name="parent">the transform for the player's hand</param>
    public void PlayerPickup(Transform parent)
    {
        // Parents the the ball to the player
        m_parent = Parent.Player;
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        m_inHand = true;

        m_rigidbody.isKinematic = true; // disables the rigidbody

        // disables the quicktime
        m_qtBase.enabled = false;
        m_qtCollider.enabled = false;
        m_collider.enabled = false;

        m_onThisFrame = true;
    }

    /// <summary>
    /// Places the ball in the dogs mouth
    /// </summary>
    /// <param name="parent">the transform for the dog's mouth</param>
    public void DogPickup(Transform parent)
    {
        if (m_parent != Parent.Player) // makes sure the dog's not trying to take it from the players hand
        {
            // Parents the ball to the dog
            m_parent = Parent.Dog;
            transform.parent = parent;
            transform.localPosition = Vector3.zero;
            m_inHand = true;

            m_rigidbody.isKinematic = true; // disables the rigidbody

            // disables the quicktime
            m_qtBase.enabled = false;
            m_qtCollider.enabled = false;
            m_collider.enabled = false;

            if (m_interactWheel)
            {
                m_interactWheel.EnableOption(0); // re-enables the ball pickup on the interact wheel
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_parent == Parent.None)
        {
            float speed = collision.relativeVelocity.magnitude;
            Debug.Log(speed.ToString());
            AkSoundEngine.SetRTPCValue("BallCollideForce", speed);
            m_ballCollideSound.Post(gameObject);
        }
    }
}
