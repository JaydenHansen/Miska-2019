using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Invokes events when objects with a certain tag enter/exit a trigger
/// </summary>
public class EventTrigger : MonoBehaviour
{
    public string m_tag;
    public VoidEvent m_onEnter;
    public VoidEvent m_onExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == m_tag) // if the object has the right tag
        {
            m_onEnter.Invoke(); // invoke the on enter event
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == m_tag) // if the object has the right tag
        {
            m_onExit.Invoke(); // invoke the on exit event
        }
    }
}
