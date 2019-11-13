using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quicktime trigger that toggles between success and failure
/// </summary>
public class QuicktimeToggle : QuicktimeTrigger
{
    public KeyCode m_startButton;
    public KeyCode m_stopButton;

    bool m_active = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        m_stayInQuicktime = true;
    }

    /// <summary>
    /// Calls the OnStart function for all responses
    /// </summary>
    protected override void QuicktimeStart()
    {
        foreach (QuicktimeResponse response in m_responses)
        {
            response.OnStart();
        }
    }

    /// <summary>
    /// On update will toggle between sucess and failure
    /// </summary>
    /// <returns></returns>
    protected override QuicktimeResult QuicktimeUpdate()
    {        
        if (!m_active && Input.GetKeyDown(m_startButton)) // if the toggle hasn't already been activated and the player presses the start button
        {
            m_active = true;
            return QuicktimeResult.Success;
        }
        else if (m_active && Input.GetKeyDown(m_stopButton)) // if the toggle has already been activated and the player presses the stop button
        {
            m_active = false;
            return QuicktimeResult.Failure;
        }        

        return QuicktimeResult.Continue; // if no buttons have been pressed continue to the next frame
    }

    /// <summary>
    /// Calls the OnSuccess on all responses
    /// </summary>
    protected override void QuicktimeSuccess()
    {
        foreach (QuicktimeResponse response in m_responses)
        {
            response.OnSuccess();
        }
    }

    /// <summary>
    /// Calls the OnFailure on all responses
    /// </summary>
    protected override void QuicktimeFailure()
    {
        foreach (QuicktimeResponse response in m_responses)
        {
            response.OnFailure();
        }
    }

    /// <summary>
    /// Exits the quicktime
    /// </summary>
    protected override void QuicktimeExit()
    {
        m_inQuicktime = false;
    }
}

