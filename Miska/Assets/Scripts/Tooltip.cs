using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Enables child objects after the user hovers their mouse over for a certain amount of time
/// </summary>
public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject[] m_children;
    public float m_threshold = 1;
    public float m_max = 1.5f;

    bool m_mouseOver = false;
    float m_count = 0;
    bool m_childrenActive;

    // Update is called once per frame
    void Update()
    {
        if (m_mouseOver) // if the mouse is over the object
        {
            m_count += Time.deltaTime; // start counting
            if (!m_childrenActive && m_count >= m_threshold) // if the timer has reached the threshold 
            {
                foreach (GameObject child in m_children) // enable all children
                {
                    child.SetActive(true);
                }
                m_childrenActive = true;
            }
        }
        else // if the mouse isn't over the object
        {
            m_count -= Time.deltaTime; // count down
            if (m_childrenActive && m_count < m_threshold) // if the timer goes under the threshold 
            {
                foreach (GameObject child in m_children) // disable all children
                {
                    child.SetActive(false);
                }
                m_childrenActive = false;
            }
        }
        m_count = Mathf.Clamp(m_count, 0, m_max); // clamp the timer between [0, m_max]
        
    }

    /// <summary>
    /// Pointer event to let the script know the mouse is over the tooltip
    /// </summary>
    /// <param name="eventData">The event data for the pointer</param>
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        m_mouseOver = true;
    }

    /// <summary>
    /// Pointer event to let the script know the mouse is no longer over the tooltip
    /// </summary>
    /// <param name="eventData">The event data for the pointer</param>
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        m_mouseOver = false;
    }
}
