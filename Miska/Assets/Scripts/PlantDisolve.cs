using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dissolves a set of plants
/// </summary>
public class PlantDisolve : MonoBehaviour
{
    public Renderer[] m_plants;
    public float m_dissolveSpeed;
    public float m_dissolveRandomOffset;

    bool m_active;
    bool m_alreadyActivated;
    float m_timer;
    float[] m_dissolveOffsets;

    // Start is called before the first frame update
    void Start()
    {
        m_dissolveOffsets = new float[m_plants.Length];
        for (int i = 0; i < m_dissolveOffsets.Length; i++)
        {
            Random.Range(-m_dissolveRandomOffset, m_dissolveRandomOffset);
        }
        foreach (Renderer plant in m_plants) // starts each plant as already dissolved
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
        if (m_active) // if the plant should be dissolving
        {
            m_timer += Time.deltaTime;

            for (int i = 0; i < m_plants.Length; i++)
            { 
                foreach (Material material in m_plants[i].materials)
                {
                    material.SetFloat("_DissolveSlider", -((m_timer / (m_dissolveSpeed + m_dissolveOffsets[i])) +1)); // lerps between the dissolve values
                }
            }

            if (m_timer > m_dissolveSpeed)
                m_active = false;
        }
    }

    /// <summary>
    /// Starts the dissolve timer
    /// </summary>
    public void StartDissolve()
    {
        if (!m_alreadyActivated) // can only be activated once
        {
            m_timer = 0;
            m_active = true;
            m_alreadyActivated = true;
        }
    }
}
