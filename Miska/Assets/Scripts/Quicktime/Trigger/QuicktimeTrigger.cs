using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuicktimeResult
{
    Success,
    Continue,
    Failure
}


public class QuicktimeTrigger : MonoBehaviour
{
    protected bool m_inQuicktime = false;
    protected bool m_stayInQuicktime = false;
    protected QuicktimeResponse[] m_responses;
    protected Player m_player;

    public Player Player
    {
        get { return m_player; }
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        m_responses = GetComponents<QuicktimeResponse>();
        foreach(QuicktimeResponse response in m_responses)
        {
            response.Owner = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_inQuicktime)
        {
            QuicktimeResult result = QuicktimeUpdate();
            switch (result)
            {
                case QuicktimeResult.Success:
                    QuicktimeSuccess();
                    if (!m_stayInQuicktime)
                        m_inQuicktime = false;
                    break;
                case QuicktimeResult.Failure:
                    QuicktimeFailure();
                    if (!m_stayInQuicktime)
                        m_inQuicktime = false;
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player)
            {
                m_player = player;
            }

            m_inQuicktime = true;
            QuicktimeStart();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_inQuicktime && other.tag == "Player")
        {
            m_inQuicktime = false;
            QuicktimeExit();
            m_player = null;
        }
    }

    protected virtual void QuicktimeStart()
    {

    }

    protected virtual QuicktimeResult QuicktimeUpdate()
    {
        return QuicktimeResult.Continue;
    }

    protected virtual void QuicktimeSuccess()
    {

    }

    protected virtual void QuicktimeFailure()
    {

    }

    protected virtual void QuicktimeExit()
    {
        m_inQuicktime = false;
    }
}
