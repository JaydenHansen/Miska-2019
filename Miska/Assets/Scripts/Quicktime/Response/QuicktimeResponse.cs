using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuicktimeResponse : MonoBehaviour
{
    protected QuicktimeTrigger m_owner;

    public QuicktimeTrigger Owner
    {
        get { return m_owner; }
        set { m_owner = value; }
    }

    public virtual void OnStart()
    {

    }

    public virtual void OnSuccess()
    {

    }

    public virtual void OnFailure()
    {

    }
}
