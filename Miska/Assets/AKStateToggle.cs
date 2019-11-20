using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script for changing Wwise States
/// </summary>
public class AKStateToggle : MonoBehaviour
{
    [Tooltip("List of states, put default (runs on scene load) at top")]
    public AK.Wwise.Event[] m_stateEvents;

    /// <summary>
    /// Sets up the default state
    /// </summary>
    void Start()
    {
        m_stateEvents[0].Post(gameObject);
    }

    /// <summary>
    /// Posts Event in an array position
    /// </summary>
    /// <param name="i">array position of event</param>
    public void PostStateEvent(int i)
    {
        if(m_stateEvents[i].IsValid())
        {
            m_stateEvents[i].Post(gameObject);
        }
    }
}
