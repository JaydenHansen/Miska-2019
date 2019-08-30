using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject[] m_children;
    public float m_threshold = 1;
    public float m_max = 1.5f;

    bool m_mouseOver = false;
    float m_count = 0;
    bool m_childrenActive;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_mouseOver)
        {
            m_count += Time.deltaTime;
            if (!m_childrenActive && m_count >= m_threshold)
            {
                foreach (GameObject child in m_children)
                {
                    child.SetActive(true);
                }
                m_childrenActive = true;
            }
        }
        else
        {
            m_count -= Time.deltaTime;
            if (m_childrenActive && m_count < m_threshold)
            {
                foreach (GameObject child in m_children)
                {
                    child.SetActive(false);
                }
                m_childrenActive = false;
            }
        }
        m_count = Mathf.Clamp(m_count, 0, m_max);
        
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        m_mouseOver = true;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        m_mouseOver = false;
    }
}
