using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;




public class QuicktimeEvents : QuicktimeResponse
{
    public PlayerEvent m_onStart;
    public PlayerEvent m_onSuccess;
    public PlayerEvent m_onFailure;


    public override void OnStart()
    {
        m_onStart.Invoke(m_owner ? m_owner.Player : null);
    }

    public override void OnSuccess()
    {
        m_onSuccess.Invoke(m_owner ? m_owner.Player : null);
    }

    public override void OnFailure()
    {
        m_onFailure.Invoke(m_owner ? m_owner.Player : null);
    }

}
