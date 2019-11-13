using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quicktime response that parents a child tranform to a parent transform
/// </summary>
public class QuicktimeAddToChildren : QuicktimeResponse
{
    public Transform m_parent;
    public Transform m_child;

    /// <summary>
    /// Changes the childs parent and resets the local position
    /// </summary>
    public override void OnSuccess()
    {
        m_child.parent = m_parent;
        m_child.localPosition = Vector3.zero;
    }
}
