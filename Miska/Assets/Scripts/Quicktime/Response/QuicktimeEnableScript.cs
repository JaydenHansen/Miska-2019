using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enables/disables a specific script
/// </summary>
public class QuicktimeEnableScript : QuicktimeResponse
{
    public MonoBehaviour m_script;

    /// <summary>
    /// Enables the script
    /// </summary>
    public override void OnSuccess()
    {
        m_script.enabled = true;
    }

    /// <summary>
    /// Disables the script
    /// </summary>
    public override void OnFailure()
    {
        m_script.enabled = false;
    }
}
