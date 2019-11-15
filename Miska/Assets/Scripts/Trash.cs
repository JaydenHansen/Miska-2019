using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls how trash is picked up and destroyed
/// </summary>
public class Trash : MonoBehaviour
{    
    public float m_delay;
    public float m_scaleThreshold;
    public bool m_disableOldParent;

    float m_timer;
    Transform m_parent;
    Transform m_target;
    bool m_pickedUp;
    Vector3 m_basePos;
    Vector3 m_baseScale;
    public AK.Wwise.Event m_pickupSound;
    //m_chipsSound

    public Transform Target
    {
        get { return m_target; }
        set { m_target = value; }
    }

    //TODO: Include system for identifying type of trash to select appropriate sound

    // Start is called before the first frame update
    void Start()
    {
        m_basePos = transform.position;
        m_baseScale = transform.localScale;        
        m_parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_pickedUp) // if the trash has been picked up
        {
            m_timer += Time.deltaTime;

            if (m_target && m_timer <= m_scaleThreshold) // if the timer hasn't reached the scale threshold
                transform.position = Vector3.Lerp(m_basePos, m_target.position, (m_timer / (m_scaleThreshold))); // lerp between the starting pos and the target pos
            else if (m_timer > m_scaleThreshold) // if the timer has reached the scale threshold
                transform.localScale = Vector3.Lerp(m_baseScale, Vector3.zero, (m_timer - m_scaleThreshold) / (m_delay - m_scaleThreshold)); // lerp the scale between the base scale and a scale of zero

            if (m_timer >= m_delay) // if the trash has finished being picked up
            {
                m_pickupSound.Post(gameObject);
                transform.parent = m_parent;
                if (m_disableOldParent)                
                    m_parent.gameObject.SetActive(false);                
                else                
                    gameObject.SetActive(false);                
            }
        }
    }

    /// <summary>
    /// Start the pickup
    /// </summary>
    /// <param name="target">The transform that the trash will lerp to</param>
    public void StartPickup(Transform target)
    {
        m_basePos = transform.position;
        m_pickedUp = true;
        m_target = target;
        transform.parent = target;
        m_baseScale = transform.localScale;
    }
}
