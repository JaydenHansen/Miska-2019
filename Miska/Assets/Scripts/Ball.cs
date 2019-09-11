using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Parent
{
    Player,
    Dog,
    None
}

public class Ball : MonoBehaviour
{
    public float m_throwStrength;
    public Transform m_cameraArm;
    public Collider m_collider;

    bool m_inHand = false;
    Rigidbody m_rigidbody;
    QuicktimeBase m_qtBase;
    bool m_onThisFrame;
    Parent m_parent;

    public bool InHand
    {
        get { return m_inHand; }
    }
    public Parent Parent
    {
        get { return m_parent; }
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
        if (m_inHand && !m_onThisFrame && m_parent == Parent.Player && Input.GetKeyDown(KeyCode.Mouse0))
        {
            m_inHand = false;
            transform.parent = null;
            m_rigidbody.isKinematic = false;
            m_rigidbody.AddForce(m_cameraArm.forward * m_throwStrength);
            m_qtBase.enabled = true;
            m_parent = Parent.None;
            m_collider.enabled = true;
        }

        m_onThisFrame = false;
    }

    public void PlayerPickup(Transform parent)
    {
        m_parent = Parent.Player;
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        m_inHand = true;
        m_rigidbody.isKinematic = true;
        m_qtBase.enabled = false;
        m_onThisFrame = true;
        m_collider.enabled = false;
    }

    public void DogPickup(Transform parent)
    {
        if (m_parent != Parent.Player)
        {
            m_parent = Parent.Dog;
            transform.parent = parent;
            transform.localPosition = Vector3.zero;
            m_inHand = true;
            m_rigidbody.isKinematic = true;
            m_collider.enabled = false;
        }
    }
}
