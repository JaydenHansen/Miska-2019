using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets the look direction of a transform
/// </summary>
public class QuicktimeSetRotation : QuicktimeResponse
{
    public Transform m_transform;
    public Vector3 m_direction;
    public bool m_localDirection;

    /// <summary>
    /// Sets the rotation of the transform to the lookrotation of the direction
    /// </summary>
    public override void OnSuccess()
    {
        m_transform.rotation = Quaternion.LookRotation(m_localDirection ? transform.rotation * m_direction : m_direction);
    }
}
