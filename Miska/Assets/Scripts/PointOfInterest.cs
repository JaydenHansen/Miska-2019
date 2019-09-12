using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointOfInterest : MonoBehaviour
{
    public PopUp m_popUp;
    public Text m_trashCount;
    public TrashCan m_trashCan;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            m_popUp.StartPopUp(IconType.Rubbish, false);
            m_trashCount.text = m_trashCan.TrashLeft + "/" + m_trashCan.m_requiredTrash;
            m_trashCount.enabled = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            m_trashCount.text = m_trashCan.TrashLeft + "/" + m_trashCan.m_requiredTrash;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            m_popUp.StopPopUp();
    }
}
