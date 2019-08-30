using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuicktimeButtonPress : QuicktimeTrigger
{
    public KeyCode m_button;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
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
        if (Input.GetKeyDown(m_button))
        {
            return QuicktimeResult.Success;
        }        

        return QuicktimeResult.Continue;
    }

    protected override void QuicktimeSuccess()
    {
        foreach(QuicktimeResponse response in m_responses)
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
}
