using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantDisolve : MonoBehaviour
{
    public Renderer[] m_plants;
    public float m_disolveSpeed;

    bool m_active;
    float m_timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_active)
        {
            m_timer += Time.deltaTime;

            foreach(Renderer plant in m_plants)
            {
                foreach (Material material in plant.materials)
                {
                    material.SetFloat("_DissolveSlider", -((m_timer / m_disolveSpeed) +1));
                }
            }

            if (m_timer > m_disolveSpeed)
                m_active = false;
        }
    }

    public void StartDissolve()
    {
        m_timer = 0;
        m_active = true;
    }
}
