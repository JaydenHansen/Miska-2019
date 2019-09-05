using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{    
    public float m_delay;

    float m_timer;
    Transform m_parent;
    bool m_pickedUp;

    // Start is called before the first frame update
    void Start()
    {
        m_timer = m_delay;
        m_parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_pickedUp)
        {
            m_timer -= Time.deltaTime;

            if (m_timer <= 0)
            {
                transform.parent = m_parent;
                m_parent.gameObject.SetActive(false);                
            }
        }
    }

    public void StartPickup()
    {
        m_pickedUp = true;
    }
}
