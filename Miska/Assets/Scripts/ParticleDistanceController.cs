using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleDistanceController : MonoBehaviour
{
    public Transform m_target;
    public float m_distance;

    bool m_active;
    ParticleSystem m_particle;

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - m_target.position).sqrMagnitude > m_distance * m_distance)
        {
            if (!m_active)
            {
                m_particle.Play();
                m_active = true;
            }
        }
        else
        {
            if (m_active)
            {
                m_particle.Stop();
                m_active = false;
            }
        }
    }
}
