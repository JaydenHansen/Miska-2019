using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuicktimeResult
{
    Success,
    Continue,
    Failure
}

public class QuicktimeBase : MonoBehaviour
{
    protected Player m_player;
    protected bool m_inQuicktime = false;
    protected QuicktimeResponse[] m_responses;

    public Player Player
    {
        get { return m_player; }
    }
    // Start is called before the first frame update
    public virtual void Start()
    {
        m_responses = GetComponents<QuicktimeResponse>();
        foreach (QuicktimeResponse response in m_responses)
        {
            response.Owner = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
