using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointOfInterest : MonoBehaviour
{
    public GameObject m_player;
    public PopUp m_popUp;
    public Text m_trashCount;
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
                    m_popUp.StartPopUp(IconType.Rubbish, false);
                    m_trashCount.text = (m_trashCan.m_requiredTrash - m_trashCan.TrashLeft) + "/" + m_trashCan.m_requiredTrash;
                    m_trashCount.enabled = true;
                }
                else
                {
                    m_popUp.StartPopUp(IconType.Bin, false);
                }
            }
            else if (m_playerInArea && !playerInArea) // Player Exit
            {
                m_popUp.StopPopUp();
                m_trashCount.enabled = false;
            }
            else if (m_playerInArea && playerInArea) // Player Stay
            {
                if (m_trashCan.TrashLeft != 0)
                {
                    m_trashCount.text = (m_trashCan.m_requiredTrash - m_trashCan.TrashLeft) + "/" + m_trashCan.m_requiredTrash;
                }
                else if (m_popUp.m_currentIcon == IconType.Rubbish)
                {
                    m_popUp.StartPopUp(IconType.Bin, false);
                    m_trashCount.enabled = false;
                }                
            }

            m_playerInArea = playerInArea;
        }
        else if (m_playerInArea)
        {
            m_popUp.StopPopUp();
            m_trashCount.enabled = false;
            m_playerInArea = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireCube(Vector3.zero, m_size);
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.white;
    }
}
