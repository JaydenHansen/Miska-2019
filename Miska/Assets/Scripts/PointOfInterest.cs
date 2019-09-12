﻿using System.Collections;
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
        bool playerInArea = false;
        Collider[] hits = Physics.OverlapBox(transform.position, m_size * 0.5f, transform.rotation, 1 << LayerMask.NameToLayer("Player"));
        foreach (Collider hit in hits)
        {
            if (hit.gameObject == m_player)
            {
                playerInArea = true;
            }
        }

        if (!m_playerInArea && playerInArea)
        {
            m_popUp.StartPopUp(IconType.Rubbish, false);
            m_trashCount.text = m_trashCan.TrashLeft + "/" + m_trashCan.m_requiredTrash;
            m_trashCount.enabled = true;
        }
        else if (m_playerInArea && !playerInArea)
        {
            m_popUp.StopPopUp();
            m_trashCount.enabled = false;
        }
        else if (m_playerInArea && playerInArea)
        {
            m_trashCount.text = m_trashCan.TrashLeft + "/" + m_trashCan.m_requiredTrash;
        }

        m_playerInArea = playerInArea;
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
