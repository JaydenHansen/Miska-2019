using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractWheel : MonoBehaviour
{
    public Image[] m_images;
    public Image m_mousePos;
    public float m_mouseSpeed;
    public float m_maxMagnitude = 40;
    public float m_deadZone = 10;
    public float m_unselectedAlpha;

    float m_angle;
    Vector2 m_mousePosition;
    int m_currentSelected;
    bool m_enabled;
    bool[] m_disabledOptions;

    // Start is called before the first frame update
    void Start()
    {
        m_angle = 360f / m_images.Length;
        m_mousePosition = Vector2.zero;
        m_disabledOptions = new bool[m_images.Length];

        foreach(Image image in m_images)
        {
            Color newColor = image.color;
            newColor.a = m_unselectedAlpha;
            image.color = newColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_mousePosition += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * m_mouseSpeed;
        m_mousePosition = Vector2.ClampMagnitude(m_mousePosition, m_maxMagnitude);
        m_mousePos.rectTransform.localPosition = m_mousePosition;

        if (m_mousePosition.magnitude > m_deadZone)
        {
            float angleBetween = -Vector2.SignedAngle(Vector2.up, m_mousePosition);
            if (angleBetween < 0)
                angleBetween += 360;

            for (int i = 0; i < m_images.Length; i++)
            {
                if (i != 0)
                {
                    if (angleBetween > (i * m_angle) - (m_angle * 0.5f) && angleBetween <= (i * m_angle) + (m_angle * 0.5f))
                    {
                        ChangeSelected(i);
                    }
                }
                else
                {
                    if ((angleBetween <= (m_angle * 0.5f)) || (angleBetween > 360 - (m_angle * 0.5f)))
                    {
                        ChangeSelected(i);
                    }
                }
            }
        }
        else
        {
            ChangeSelected(-1);
        }
    }

    void ChangeSelected(int newSelected)
    {
        if (m_currentSelected == newSelected)
            return;

        if (m_currentSelected >= 0)
        {
            Color newColor = m_images[m_currentSelected].color;
            newColor.a = m_unselectedAlpha;
            m_images[m_currentSelected].color = newColor;
        }

        m_currentSelected = newSelected;

        if (m_currentSelected >= 0 && !m_disabledOptions[m_currentSelected])
        {
            Color newColor = m_images[m_currentSelected].color;
            newColor.a = 1;
            m_images[m_currentSelected].color = newColor;
        }
    }

    public void EnableWheel()
    {
        gameObject.SetActive(true);
        ChangeSelected(-1);
        m_mousePosition = Vector2.zero;
        m_mousePos.rectTransform.localPosition = m_mousePosition;
        m_enabled = true;
    }

    public void DisableWheel()
    {
        if (m_enabled)
        {
            if (m_currentSelected >= 0 && !m_disabledOptions[m_currentSelected])
            {
                QuicktimeResponse[] responses = m_images[m_currentSelected].GetComponents<QuicktimeResponse>();
                foreach (QuicktimeResponse response in responses)
                {
                    response.OnSuccess();
                }
            }

            gameObject.SetActive(false);
            m_enabled = false;
        }
    }

    public void DisableOption(int index)
    {
        if (index >= 0 && index < m_disabledOptions.Length)
        {
            m_disabledOptions[index] = true;
        }
        else
        {
            Debug.Log("Disabling Option out of bounds");
        }
    }

    public void EnableOption(int index)
    {
        if (index >= 0 && index < m_disabledOptions.Length)
        {
            m_disabledOptions[index] = false;
        }
        else
        {
            Debug.Log("Enabling Option out of bounds");
        }
    }
}
