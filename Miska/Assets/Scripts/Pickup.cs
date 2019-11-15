using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes a rigid body follow a target
/// Unused
/// </summary>
public class Pickup : MonoBehaviour
{
    protected Transform m_target;
    protected Rigidbody m_rigidbody;
    protected Collider m_collider;
    protected float m_baseDrag;
    protected float m_baseAngularDrag;
    protected Renderer m_renderer;

    public Transform Target
    {
        get { return m_target; }
    }
    public Rigidbody Rigidbody
    {
        get { return m_rigidbody; }
    }
    public Collider Collider
    {
        get { return m_collider; }
    }
    public Renderer Renderer
    {
        get { return m_renderer; }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();
        m_renderer = GetComponent<Renderer>();
        m_baseDrag = m_rigidbody.drag;
        m_baseAngularDrag = m_rigidbody.angularDrag;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_target != null)
        {
            m_rigidbody.drag = 10 - Mathf.Clamp((transform.position - m_target.position).magnitude, 0, 10); // increases the drag the closer the rigidbody is to the collider
            m_rigidbody.AddForce((m_target.position - transform.position) * 100);
        }
    }

    /// <summary>
    /// Starts the object following the target
    /// </summary>
    /// <param name="target">the target to follow</param>
    public void StartPickup(Transform target)
    {
        m_target = target;
        m_rigidbody.useGravity = false;
        m_rigidbody.angularDrag = 5f;
    }

    /// <summary>
    /// Stops following the target
    /// </summary>
    public void EndPickup()
    {
        m_target = null;
        m_rigidbody.drag = m_baseDrag;
        m_rigidbody.useGravity = true;
        m_rigidbody.angularDrag = m_baseAngularDrag;
    }
}
