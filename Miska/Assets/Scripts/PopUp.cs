using System.Collections;
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
    public Sprite[] m_icons;
    public IconType m_currentIcon;
    public bool m_tempFlash;

    float m_popUpTimer;
    float m_flashTimer;
    bool m_active;
    Image m_image;
    bool m_spriteToggle;

    // Start is called before the first frame update
    void Start()
    {
        m_image = GetComponent<Image>();
        //StartPopUp(IconType.Bench, false);        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_active)
        {           
            m_flashTimer += Time.deltaTime;

            //Color color = m_image.color;
            //color.a = 1 - ((Mathf.Cos((m_timer / m_popUpTime) * Mathf.PI * m_flashAmount * 2) + 1) * 0.5f);
            //m_image.color = color;

            if (m_flashTimer >= m_flashDelay)
            {
                m_flashTimer = 0;
                m_image.sprite = m_icons[m_spriteToggle ? IconIndex : IconIndex + 1];
                m_spriteToggle = !m_spriteToggle;
            }

            if (m_tempFlash)
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

    public void StartPopUp(IconType icon, bool temp)
    {
        m_currentIcon = icon;
        m_image.sprite = m_icons[IconIndex];
        m_tempFlash = temp;

        m_popUpTimer = 0;
        
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
