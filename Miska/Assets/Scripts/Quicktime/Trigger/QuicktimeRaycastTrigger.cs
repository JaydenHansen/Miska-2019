using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Raycast trigger that activates when the player is looking at it and left clicks
/// </summary>
public class QuicktimeRaycastTrigger : QuicktimeBase
{        
    public bool m_failOnStopLooking;

    // Update is called once per frame
    void Update()
    {
        if (m_inQuicktime)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                foreach (QuicktimeResponse response in m_responses)
                {
                    response.OnSuccess();
                }
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                foreach (QuicktimeResponse response in m_responses)
                {
                    response.OnFailure();
                }
            }
        }
    }

    /// <summary>
    /// Enable the trigger. Called by the raycaster when the player is looking at the attached object
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool StartLookAt(Player player)
    {
        if (!enabled) // if the trigger is not enabled
            return false;

        if (!m_player && player) // if the script doesn't already have a player
            m_player = player; // cache the player
        m_inQuicktime = true;

        return true;
    }

    /// <summary>
    /// Disable the trigger
    /// </summary>
    public void StopLookAt()
    {
        m_inQuicktime = false;

        if (m_failOnStopLooking) // if the quicktime should fail when the player looks away
        {
            // Call the OnFailure for each response
            foreach (QuicktimeResponse response in m_responses)
            {
                response.OnFailure();
            }
        }
    }
}
