using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (m_pickedUp)
        {
            m_timer += Time.deltaTime;

            if (m_target && m_timer <= m_scaleThreshold)
                transform.position = Vector3.Lerp(m_basePos, m_target.position, (m_timer / (m_scaleThreshold)));
            else if (m_timer > m_scaleThreshold)
                transform.localScale = Vector3.Lerp(m_baseScale, Vector3.zero, (m_timer - m_scaleThreshold) / (m_delay - m_scaleThreshold));

            if (m_timer >= m_delay)
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

    public void StartPickup(Transform target)
    {
        m_pickedUp = true;
        m_target = target;
        transform.parent = target;
        m_baseScale = transform.localScale;
    }
}
