using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDissolve : MonoBehaviour
{
    public Image[] m_image;
    public float m_dissolveSpeed;
    public float m_dissolveRandomOffset;
    public Vector2 m_range;

    bool m_active;
    bool m_alreadyActivated;
    float m_timer;
    bool m_direction;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Image image in m_image)
        {
            image.material.SetFloat("_DissolveSlider", 0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_active)
        {
            m_timer += Time.deltaTime;

            for (int i = 0; i < m_image.Length; i++)
            {
                Random.InitState(i);
                float newValue = Remap(m_timer / (m_dissolveSpeed + Random.Range(-m_dissolveRandomOffset, m_dissolveRandomOffset)), 0, 1, m_range.x, m_range.y) * (m_direction ? 1 : -1);
                m_image[i].material.SetFloat("_DissolveSlider", newValue);                
            }

            if (m_timer > m_dissolveSpeed)
                m_active = false;
        }
    }

    public void StartDissolve(bool direction)
    {
        if (!m_alreadyActivated)
        {
            m_direction = direction;
            m_timer = 0;
            m_active = true;
            m_alreadyActivated = true;
        }
    }

    float Remap(float value, float from1, float from2, float to1, float to2)
    {
        return to1 + (value - from1) * (to2 - to1) / (from2 - from1);
    }
}

