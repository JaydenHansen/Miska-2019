using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDissolve : MonoBehaviour
{
    public Image[] m_image;
    public float m_dissolveSpeed;
    public float m_dissolveRandomOffset;

    bool m_active;
    bool m_alreadyActivated;
    float m_timer;
    bool m_direction;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Image image in m_image)
        {
            image.material.SetFloat("_DissolveSlider", 60);
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
                m_image[i].material.SetFloat("_DissolveSlider", ((m_timer / (m_dissolveSpeed + Random.Range(-m_dissolveRandomOffset, m_dissolveRandomOffset))) - 0.5f) * 120 * (m_direction ? -1 : 1));                
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
}

