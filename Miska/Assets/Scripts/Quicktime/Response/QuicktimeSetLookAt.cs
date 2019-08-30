using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuicktimeSetLookAt : QuicktimeResponse
{
    public Transform m_target;
    public CameraController m_camera;
    public LookAtMode m_lookMode;
    public float m_lookAtStrength;

    bool m_active;

    // Start is called before the first frame update
    void Start()
    {        
    }

    // Update is called once per frame
    void Update()
    {        
        if (m_active)
        {
            if (m_camera.m_lookAt == null)
            {
                m_camera.DisableControl = false;
            }
        }
    }

    public override void OnSuccess()
    {
        m_camera.m_lookAt = m_target;
        m_camera.LookMode = m_lookMode;
        m_camera.DisableControl = true;
        m_camera.LookAtStrength = m_lookAtStrength;

        m_active = true;
    }

    public override void OnFailure()
    {
        m_camera.m_lookAt = null;
        m_camera.LookMode = LookAtMode.Continuous;
        m_camera.DisableControl = false;
        m_camera.LookAtStrength = m_camera.m_baseLookAtStrength;

        m_active = false;
    }
}
