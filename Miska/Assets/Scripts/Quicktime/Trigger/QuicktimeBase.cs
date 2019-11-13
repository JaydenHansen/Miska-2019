using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuicktimeResult
{
    Success,
    Continue,
    Failure
}

/// <summary>
/// Base class for quicktime triggers
/// simply stores the quicktime responses attached to the gameobject
/// </summary>
public class QuicktimeBase : MonoBehaviour
{
    protected Player m_player;
    protected bool m_inQuicktime = false;
    protected QuicktimeResponse[] m_responses;

    public Player Player
    {
        get { return m_player; }
    }

    /// <summary>
    /// base start function that stores attached responses
    /// </summary>
    public virtual void Start()
    {
        m_responses = GetComponents<QuicktimeResponse>();
        foreach (QuicktimeResponse response in m_responses)
        {
            response.Owner = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
