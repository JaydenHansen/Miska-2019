using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointOfInterest : MonoBehaviour
{
    public GameObject m_player;
    public HUD_UI m_hudUI;
    public TrashCan m_trashCan;
    public Vector3 m_size;

    bool m_playerInArea;

    private void Update()
    {
        if (!m_trashCan.Triggered)
        {
            bool playerInArea = false;
            Collider[] hits = Physics.OverlapBox(transform.position, m_size * 0.5f, transform.rotation, 1 << LayerMask.NameToLayer("Player"));
            foreach (Collider hit in hits)
            {
                if (hit.gameObject == m_player)
                {
                    playerInArea = true;
                }
            }

            if (!m_playerInArea && playerInArea) // Player Enter
            {
                if (m_trashCan.TrashLeft != 0)
                {
                    m_hudUI.SetupTrashScene(m_trashCan.m_requiredTrash, m_trashCan.TrashLeft);
                }
                else
                {
                    m_hudUI.SetupDisposeScene();
                }
            }
            else if (m_playerInArea && !playerInArea) // Player Exit
            {
                m_hudUI.OnTrashAreaExit();
            }
            //else if (m_playerInArea && playerInArea) // Player Stay
            //{
            //    if (m_trashCan.TrashLeft != 0)
            //    {
            //        m_trashCount.text = (m_trashCan.m_requiredTrash - m_trashCan.TrashLeft) + "/" + m_trashCan.m_requiredTrash;
            //    }              
            //}

            m_playerInArea = playerInArea;
        }
        else if (m_playerInArea)
        {
            m_hudUI.OnAllTrashDisposed();
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
    //    Gizmos.DrawWireCube(Vector3.zero, m_size);
    //    Gizmos.matrix = Matrix4x4.identity;
    //    Gizmos.color = Color.white;
    //}
}
