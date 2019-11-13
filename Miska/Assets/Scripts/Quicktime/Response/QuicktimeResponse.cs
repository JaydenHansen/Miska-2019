using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for quicktime responses
/// </summary>
public class QuicktimeResponse : MonoBehaviour
{
    protected QuicktimeBase m_owner;

    public QuicktimeBase Owner
    {
        get { return m_owner; }
        set { m_owner = value; }
    }

    /// <summary>
    /// Base function for what to do when the quicktime starts
    /// </summary>
    public virtual void OnStart()
    {

    }

    /// <summary>
    /// Base function for what to do when the quicktime succeeds
    /// </summary>
    public virtual void OnSuccess()
    {

    }

    /// <summary>
    /// Base function for what to do when the quicktime fails
    /// </summary>
    public virtual void OnFailure()
    {

    }
}
