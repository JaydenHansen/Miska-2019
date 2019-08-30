using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{    
    public float m_delay;

    float m_timer;
    Transform m_parent;



    // Start is called before the first frame update
    void Start()
    {
        m_timer = m_delay;
        m_parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent != m_parent)
        {
            m_timer -= Time.deltaTime;

            if (m_timer <= 0)
            {

                Destroy(m_parent.gameObject);

                Destroy(gameObject);
            }
        }
    }
}
