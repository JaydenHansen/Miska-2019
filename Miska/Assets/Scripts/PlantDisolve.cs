using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantDisolve : MonoBehaviour
{
    public Renderer[] m_plants;
    public float m_dissolveSpeed;
    public float m_dissolveRandomOffset;

    bool m_active;
    bool m_alreadyActivated;
    float m_timer;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Renderer plant in m_plants)
        {
            foreach (Material material in plant.materials)
            {
                material.SetFloat("_DissolveSlider", 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_active)
        {
            m_timer += Time.deltaTime;

            for (int i = 0; i < m_plants.Length; i++)
            { 
                Random.InitState(i);
                foreach (Material material in m_plants[i].materials)
                {
                    material.SetFloat("_DissolveSlider", -((m_timer / (m_dissolveSpeed + Random.Range(-m_dissolveRandomOffset, m_dissolveRandomOffset))) +1));
                }
            }

            if (m_timer > m_dissolveSpeed)
                m_active = false;
        }
    }

    public void StartDissolve()
    {
        if (!m_alreadyActivated)
        {
            m_timer = 0;
            m_active = true;
            m_alreadyActivated = true;
        }
    }
}
