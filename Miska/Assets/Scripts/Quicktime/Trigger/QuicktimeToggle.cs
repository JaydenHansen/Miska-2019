using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected override void QuicktimeStart()
    {
        foreach (QuicktimeResponse response in m_responses)
        {
            response.OnStart();
        }
    }

    protected override QuicktimeResult QuicktimeUpdate()
    {        
        if (!m_active && Input.GetKeyDown(m_startButton))
        {
            m_active = true;
            return QuicktimeResult.Success;
        }
        else if (m_active && Input.GetKeyDown(m_stopButton))
        {
            m_active = false;
            return QuicktimeResult.Failure;
        }        

        return QuicktimeResult.Continue;
    }

    protected override void QuicktimeSuccess()
    {
        foreach (QuicktimeResponse response in m_responses)
        {
            response.OnSuccess();
        }
    }

    protected override void QuicktimeFailure()
    {
        foreach (QuicktimeResponse response in m_responses)
        {
            response.OnFailure();
        }
    }

    protected override void QuicktimeExit()
    {
        m_inQuicktime = false;
    }
}

