using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuicktimeRaycaster : MonoBehaviour
{
    public CameraController m_cameraController;
    public float m_maxDistance;
    public LayerMask m_layers;
    public Player m_player;

    QuicktimeRaycastTrigger m_lastTrigger;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_cameraController.DirectionRay, out hit, m_maxDistance, m_layers))
        {
            QuicktimeRaycastTrigger trigger = hit.collider.GetComponent<QuicktimeRaycastTrigger>();
            if (trigger)
            {
                if (m_lastTrigger && m_lastTrigger != trigger)
                {
                    m_lastTrigger.StopLookAt();
                    trigger.StartLookAt(m_player);
                    m_lastTrigger = trigger;
                }
                else
                {
                    trigger.StartLookAt(m_player);
                    m_lastTrigger = trigger;
                }
                
            }
        }
        else
        {
            if (m_lastTrigger)
            {
                m_lastTrigger.StopLookAt();
                m_lastTrigger = null;
            }
        }
    }
}
