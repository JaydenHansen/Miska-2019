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
    public bool m_useDistance;
    [Tooltip("X: Distance where image is full Y: Distance where image is gone")]
    public Vector2 m_dissolveDistance;
    public Transform m_player;
    public Transform m_cameraArm;
    public bool m_orbit;
    public float m_orbitRadius;
    public Transform m_orbitTransform;

    bool m_active;
    bool m_alreadyActivated;
    float m_timer;
    bool m_direction;

    public bool UseDistance
    {
        get { return m_useDistance; }
        set { m_useDistance = value; }
    }

    // Start is called before the first frame updated
    void Start()
    {
        
        foreach (Image image in m_image)
        {
            image.material = new Material(image.material);
            image.material.name += " (Instance)";
            image.material.SetFloat("_DissolveSlider", 0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = m_cameraArm.forward;

        if (m_orbit)
        {
            transform.position = m_orbitTransform.position + ((m_cameraArm.position - transform.position).normalized * m_orbitRadius);
        }

        if (m_useDistance)
        {
            float distance = (transform.position - m_player.position).magnitude;
            for (int i = 0; i < m_image.Length; i++)
            {
                float newValue = Remap(1f - Mathf.Clamp01((distance - m_dissolveDistance.x) / (m_dissolveDistance.y - m_dissolveDistance.x)), 0, 1, m_range.x, m_range.y);
                m_image[i].material.SetFloat("_DissolveSlider", newValue);
            }
        }
        else
        {
            if (m_active)
            {
                m_timer += Time.deltaTime;

                for (int i = 0; i < m_image.Length; i++)
                {
                    Random.InitState(i);
                    float newValue;
                    if (m_direction)                   
                        newValue = Remap(Mathf.Clamp01(m_timer / (m_dissolveSpeed + Random.Range(-m_dissolveRandomOffset, m_dissolveRandomOffset))), 0, 1, m_range.x, m_range.y);
                    else
                        newValue = Remap(1 - Mathf.Clamp01(m_timer / (m_dissolveSpeed + Random.Range(-m_dissolveRandomOffset, m_dissolveRandomOffset))), 0, 1, m_range.x, m_range.y);
                    m_image[i].material.SetFloat("_DissolveSlider", newValue);
                }

                if (m_timer > m_dissolveSpeed)
                    m_active = false;
            }
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

    private void OnDrawGizmosSelected()
    {
        if (m_orbit && m_orbitTransform)
            Gizmos.DrawWireSphere(m_orbitTransform.position, m_orbitRadius);
    }
}

