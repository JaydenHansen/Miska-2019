using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Bookmark : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float m_speed;
    public float m_xOffset;

    RectTransform m_rect;
    float m_startX;
    float m_finalX;
    bool m_mouseOver;
    float m_timer;

    void Start()
    {
        m_rect = GetComponent<RectTransform>();
        m_startX = m_rect.localPosition.x;
        m_finalX = m_startX + m_xOffset;
    }

    void Update()
    {
        if (m_mouseOver)
            m_timer += Time.deltaTime;
        else
            m_timer -= Time.deltaTime;

        m_timer = Mathf.Clamp(m_timer, 0, m_speed);

        m_rect.localPosition = new Vector3(Mathf.Lerp(m_startX, m_finalX, m_timer / m_speed), m_rect.localPosition.y, m_rect.localPosition.z);
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
