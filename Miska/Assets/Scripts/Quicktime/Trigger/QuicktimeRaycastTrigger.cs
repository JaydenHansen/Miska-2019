using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuicktimeRaycastTrigger : QuicktimeBase
{         
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
        }
    }

    public void StartLookAt(Player player)
    {
        if (!m_player && player)
            m_player = player;
        m_inQuicktime = true;
    }

    public void StopLookAt()
    {
        m_inQuicktime = false;
    }
}
