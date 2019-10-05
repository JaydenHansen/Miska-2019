using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    public string m_tag;
    public VoidEvent m_onEnter;
    public VoidEvent m_onExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == m_tag)
        {
            m_onEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == m_tag)
        {
            m_onExit.Invoke();
        }
    }
}
