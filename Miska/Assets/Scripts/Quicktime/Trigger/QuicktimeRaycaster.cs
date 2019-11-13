using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sends out raycasts to attempt to activate the opposing raycast Trigger
/// </summary>
public class QuicktimeRaycaster : MonoBehaviour
{
    public CameraController m_cameraController;
    public float m_maxDistance;
    public LayerMask m_layers; // the layers to check for
    public Player m_player;

    QuicktimeRaycastTrigger m_lastTrigger; // the last activated raycast trigger

    /// <summary>
    /// Checks for raycast triggers hit by the raycast and will activate them when found
    /// </summary>
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_cameraController.DirectionRay, out hit, m_maxDistance, m_layers)) // sends a raycast directly out of the camera
        {
            QuicktimeRaycastTrigger trigger = hit.collider.GetComponent<QuicktimeRaycastTrigger>();
            if (trigger) // checks if there is a trigger attached to the hit object
            {
                if (m_lastTrigger && m_lastTrigger != trigger) // if the player was already looking at a trigger and has now changed which one they are looking at
                {
                    m_lastTrigger.StopLookAt(); // stop the old trigger
                    if (trigger.StartLookAt(m_player)) // starts the new trigger
                        m_lastTrigger = trigger; // caches the new trigger
                }
                else
                {                    
                    if (trigger.StartLookAt(m_player))
                        m_lastTrigger = trigger;
                }
                
            }
            else if (m_lastTrigger) // if there is no trigger on the object and the player was looking at one previously
            {
                m_lastTrigger.StopLookAt(); // stop the old trigger
                m_lastTrigger = null;
            }
        }
        else // if the raycast doesn't hit anything
        {
            if (m_lastTrigger) // if the player was looking at a trigger
            {
                m_lastTrigger.StopLookAt(); // stop the old trigger
                m_lastTrigger = null;
            }
        }
    }
}
