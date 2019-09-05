﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleRock : MonoBehaviour
{
    public Vector3 m_knockOverDirection;
    public float m_knockOverStrength;
    public float m_delay;

    Rigidbody m_rigidbody;
    bool m_pickedUp;
    float m_timer;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_pickedUp)
        {
            m_timer += Time.deltaTime;
            if (m_timer > m_delay)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void KnockOver()
    {
        m_rigidbody.isKinematic = false;
        m_rigidbody.AddForce(m_knockOverDirection * m_knockOverStrength);
    }
    public void Pickup()
    {
        m_pickedUp = true;
        m_rigidbody.isKinematic = true;
    }

}
