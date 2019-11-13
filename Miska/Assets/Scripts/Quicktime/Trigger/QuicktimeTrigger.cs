using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quicktime trigger that starts and stops the quicktime depending on if the player is in an attached trigger
/// </summary>
public class QuicktimeTrigger : QuicktimeBase
{    
    protected bool m_stayInQuicktime = false;        

    /// <summary>
    /// Handles updating the quicktime for inherited classes and the success and failure of that update
    /// </summary>
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

    /// <summary>
    /// Starts the quicktime if the player enters the trigger
    /// </summary>
    /// <param name="other">The other collider in the collision</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // checks if the other collider has an attached player script and caches it
            Player player = other.GetComponent<Player>();
            if (player)
            {
                m_player = player;
            }

            m_inQuicktime = true;
            QuicktimeStart();
        }
    }

    /// <summary>
    /// Stops the quicktime if the player leaves the trigger
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (m_inQuicktime && other.tag == "Player")
        {
            m_inQuicktime = false;
            QuicktimeExit();
            m_player = null;
        }
    }

    /// <summary>
    /// Base function for when the quicktime starts
    /// </summary>
    protected virtual void QuicktimeStart()
    {

    }

    /// <summary>
    /// Base function for updating the quicktime status
    /// </summary>
    /// <returns>Returns the status of the quicktime e.g. if the quicktime has been activated or not</returns>
    protected virtual QuicktimeResult QuicktimeUpdate()
    {
        return QuicktimeResult.Continue;
    }

    /// <summary>
    /// Base function for what happens when the quicktime succeeds
    /// </summary>
    protected virtual void QuicktimeSuccess()
    {

    }

    /// <summary>
    /// Base function for what happens when the quicktime fails
    /// </summary>
    protected virtual void QuicktimeFailure()
    {

    }

    /// <summary>
    /// Base function for when the player exits the quicktime
    /// </summary>
    protected virtual void QuicktimeExit()
    {
        m_inQuicktime = false;
    }
}
