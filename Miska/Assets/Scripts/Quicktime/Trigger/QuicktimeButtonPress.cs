using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quicktime trigger that activates on a button press
/// </summary>
public class QuicktimeButtonPress : QuicktimeTrigger
{
    public KeyCode m_button;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// Calls the on start function of all responses
    /// </summary>
    protected override void QuicktimeStart()
    {
        foreach (QuicktimeResponse response in m_responses)
        {
            response.OnStart();
        }
    }

    /// <summary>
    /// Checks for if the player has pressed the corresponding button
    /// </summary>
    /// <returns>If the player presses the button will return success otherwise it will continue to the next frame</returns>
    protected override QuicktimeResult QuicktimeUpdate()
    {
        if (Input.GetKeyDown(m_button))
        {
            return QuicktimeResult.Success;
        }        

        return QuicktimeResult.Continue;
    }

    /// <summary>
    /// Calls the OnSuccess function of all responses
    /// </summary>
    protected override void QuicktimeSuccess()
    {
        foreach(QuicktimeResponse response in m_responses)
        {
            response.OnSuccess();
        }
    }

    /// <summary>
    /// Calls the OnFailure function of all responses
    /// </summary>
    protected override void QuicktimeFailure()
    {
        foreach (QuicktimeResponse response in m_responses)
        {
            response.OnFailure();
        }
    }
}
