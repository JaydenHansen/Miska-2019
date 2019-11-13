using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets the cameras' look at target
/// </summary>
public class QuicktimeSetLookAt : QuicktimeResponse
{
    public Transform m_target;
    public CameraController m_camera;
    public LookAtMode m_lookMode;
    public float m_lookAtStrength;

    bool m_active;

    /// <summary>
    /// Checks if the camera has finished looking at the target and re-enables the control
    /// </summary>
    void Update()
    {        
        if (m_active)
        {
            if (m_camera.m_lookAt == null) // if lookAt is null the camera has finished looking at the target
            {
                m_camera.DisableControl = false;
            }
        }
    }

    /// <summary>
    /// Sets the look at of the camera and disables the movement of the camera
    /// </summary>
    public override void OnSuccess()
    {
        m_camera.m_lookAt = m_target;
        m_camera.LookMode = m_lookMode;
        m_camera.DisableControl = true;
        m_camera.LookAtStrength = m_lookAtStrength;

        m_active = true;
    }

    /// <summary>
    /// Resets the camera look strength and enables the movement of the camera
    /// </summary>
    public override void OnFailure()
    {
        m_camera.m_lookAt = null;
        m_camera.LookMode = LookAtMode.Continuous;
        m_camera.DisableControl = false;
        m_camera.LookAtStrength = m_camera.m_baseLookAtStrength;

        m_active = false;
    }
}
