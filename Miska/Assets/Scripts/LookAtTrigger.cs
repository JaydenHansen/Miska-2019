using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTrigger : MonoBehaviour
{
    public GameObject m_target;
    public CameraController m_camera;
    public float m_lookAtStrength;

    void Start()
    {       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enabled)
        {
            if (other.tag == "Player")
            {
                m_camera.LookAtStrength = m_lookAtStrength;
                m_camera.m_lookAt = m_target.transform;
            }
        }
    }    

    private void OnTriggerExit(Collider other)
    {
        if (enabled)
        {
            if (other.tag == "Player")
            {
                m_camera.LookAtStrength = m_camera.m_baseLookAtStrength;
                m_camera.m_lookAt = null;
            }
        }
    }
}
