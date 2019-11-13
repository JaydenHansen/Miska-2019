using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Uses unity events for each of the outcomes
/// </summary>
public class QuicktimeEvents : QuicktimeResponse
{
    public PlayerEvent m_onStart;
    public PlayerEvent m_onSuccess;
    public PlayerEvent m_onFailure;

    /// <summary>
    /// invokes the onstart event
    /// </summary>
    public override void OnStart()
    {
        m_onStart.Invoke(m_owner ? m_owner.Player : null);
    }

    /// <summary>
    /// invokes the onsuccess event
    /// </summary>
    public override void OnSuccess()
    {
        m_onSuccess.Invoke(m_owner ? m_owner.Player : null);
    }

    /// <summary>
    /// invokes the onfailure event
    /// </summary>
    public override void OnFailure()
    {
        m_onFailure.Invoke(m_owner ? m_owner.Player : null);
    }

}
