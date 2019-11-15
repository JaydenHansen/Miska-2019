using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// OLD SCRIPT
/// UNUSED
/// </summary>
public class PopUpTrigger : MonoBehaviour
{
    public PopUp m_popUp;
    public IconType m_icon;
    public string m_tag;
    public bool m_tempFlash;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == m_tag)
        {
            m_popUp.StartPopUp(m_icon, m_tempFlash);
        }
    }
}
