using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public bool StartLookAt(Player player)
    {
        if (!enabled)
            return false;

        if (!m_player && player)
            m_player = player;
        m_inQuicktime = true;

        return true;
    }

    public void StopLookAt()
    {
        m_inQuicktime = false;

        if (m_failOnStopLooking)
        {
            foreach (QuicktimeResponse response in m_responses)
            {
                response.OnFailure();
            }
        }
    }
}
