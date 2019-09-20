﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum IconType
{
    Bench = 0,
    Bin,
    Collect,
    Handbook,
    Interact,
    Jump,
    Rubbish
};

public class PopUp : MonoBehaviour
{
    public float m_popUpTime;
    public float m_flashDelay;
    public float m_scaleSpeed;
    public float m_scaleSize;
    public Sprite[] m_icons;
    public IconType m_currentIcon;
    public bool m_useTimer;
    public bool m_autoStart;

    float m_popUpTimer;
    float m_flashTimer;
    float m_scaleTimer;
    float m_baseScale;
    bool m_active;
    Image m_image;
    Text m_text;
    bool m_spriteToggle;

    // Start is called before the first frame update
    void Start()
    {
        m_image = GetComponent<Image>();
        m_text = GetComponentInChildren<Text>();
        m_baseScale = m_image.rectTransform.sizeDelta.x;

        if (m_autoStart)
            StartPopUp(m_currentIcon, m_useTimer);        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_active)
        {           
            m_flashTimer += Time.deltaTime;
            m_scaleTimer += Time.deltaTime;

            if (m_scaleTimer <= m_scaleSpeed)
            {
                m_image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(m_baseScale, m_scaleSize, Mathf.Sin(m_scaleTimer / m_scaleSpeed * Mathf.PI)));
                m_image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(m_baseScale, m_scaleSize, Mathf.Sin(m_scaleTimer / m_scaleSpeed * Mathf.PI)));

                m_text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(m_baseScale, m_scaleSize, Mathf.Sin(m_scaleTimer / m_scaleSpeed * Mathf.PI)));
                m_text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(m_baseScale, m_scaleSize, Mathf.Sin(m_scaleTimer / m_scaleSpeed * Mathf.PI)));
            }

            //Color color = m_image.color;
            //color.a = 1 - ((Mathf.Cos((m_timer / m_popUpTime) * Mathf.PI * m_flashAmount * 2) + 1) * 0.5f);
            //m_image.color = color;

            if (m_flashTimer >= m_flashDelay)
            {
                m_flashTimer = 0;
                m_image.sprite = m_icons[m_spriteToggle ? IconIndex : IconIndex + 1];
                m_spriteToggle = !m_spriteToggle;
            }

            if (m_useTimer)
            {
                m_popUpTimer += Time.deltaTime;
                if (m_popUpTimer >= m_popUpTime)
                {
                    m_image.enabled = false;
                    m_active = false;
                    m_popUpTimer = 0;
                    m_flashTimer = 0;
                }
            }
        }
    }

    public void StartPopUp(IconType icon, bool useTimer)
    {
        m_currentIcon = icon;
        m_image.sprite = m_icons[IconIndex];
        m_useTimer = useTimer;

        m_popUpTimer = 0;
        m_scaleTimer = 0;
        
        m_active = true;
        m_image.enabled = true;
    }
    public void StopPopUp()
    {
        m_image.enabled = false;
        m_active = false;
        m_popUpTimer = 0;
        m_flashTimer = 0;
    }

    int IconIndex
    {
        get { return (int)m_currentIcon * 2; }
    }
}
