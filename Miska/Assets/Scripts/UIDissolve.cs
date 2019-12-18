using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Disolves ui images depending on the players position or a timer
/// will also pulse the scale of the canvas
/// </summary>
public class UIDissolve : MonoBehaviour
{
    public Image[] m_image;
    public float m_dissolveSpeed;
    public float m_dissolveRandomOffset;
    public bool m_startAppeared;
    public bool m_scalePulse;
    public float m_scalePulseSpeed;
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
    float m_scalePulseTimer;
    bool m_scalePulseDirection;
    Vector3 m_baseScale;
    Vector3 m_minScale;
    bool m_direction;

    public bool UseDistance
    {
        get { return m_useDistance; }
        set { m_useDistance = value; }
    }

    // Start is called before the first frame updated
    void Start()
    {
        m_baseScale = transform.localScale;
        m_minScale = m_baseScale * 0.5f;
        foreach (Image image in m_image)
        {
            image.material = new Material(image.material);
            image.material.name += " (Instance)";
            image.material.SetFloat("_DissolveSlider", m_startAppeared ? m_range.y : m_range.x);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = m_cameraArm.forward; // billboard

        if (m_scalePulse) // if the scale should pulse
        {
            if (m_scalePulseDirection) // count up
            {
                m_scalePulseTimer += Time.deltaTime;
                if (m_scalePulseTimer >= m_scalePulseSpeed) // if the timer reaches the speed value
                    m_scalePulseDirection = false; // reverse the direction
            }
            else // count down
            {
                m_scalePulseTimer -= Time.deltaTime;
                if (m_scalePulseTimer <= 0)
                    m_scalePulseDirection = true;
            }    

            transform.localScale = Vector3.Lerp(m_minScale, m_baseScale, m_scalePulseTimer / m_scalePulseSpeed); // lerp between the min and base scale using the scale timer
        }

        if (m_orbit) // if the transform should orbit
        {
            transform.position = m_orbitTransform.position + ((m_cameraArm.position - m_orbitTransform.position).normalized * m_orbitRadius); // orbit around a tranform in the direction of the player
        }

        if (m_useDistance) // if the player's distance to the object should be used to dissolve
        {
            float distance = (transform.position - m_player.position).magnitude; // the distance to the player
            for (int i = 0; i < m_image.Length; i++) // for each image
            {
                // clamps the distance between the min and map distance and remaps the value between the values for the dissolve
                float newValue = Remap(1f - Mathf.Clamp01((distance - m_dissolveDistance.x) / (m_dissolveDistance.y - m_dissolveDistance.x)), 0, 1, m_range.x, m_range.y);
                m_image[i].material.SetFloat("_DissolveSlider", newValue); // sets the dissolve value of the material
            }
        }
        else // if the dissolve works off a timer
        {
            if (m_active)
            {
                m_timer += Time.deltaTime;

                for (int i = 0; i < m_image.Length; i++)
                {
                    Random.InitState(i); // random offset for each image
                    float newValue;
                    // gets the timer / speed and remaps the value between the values for the dissolve
                    if (m_direction)
                        newValue = Remap(Mathf.Clamp01(m_timer / (m_dissolveSpeed + Random.Range(-m_dissolveRandomOffset, m_dissolveRandomOffset))), 0, 1, m_range.x, m_range.y);
                    else
                        newValue = Remap(1 - Mathf.Clamp01(m_timer / (m_dissolveSpeed + Random.Range(-m_dissolveRandomOffset, m_dissolveRandomOffset))), 0, 1, m_range.x, m_range.y);
                    m_image[i].material.SetFloat("_DissolveSlider", newValue); // sets the dissolve value of the material
                }

                if (m_timer > m_dissolveSpeed)
                    m_active = false;
            }
        }
    }

    /// <summary>
    /// Start dissolving the images using a timer
    /// </summary>
    /// <param name="direction">The direction the dissolve should go in (true=start appearing, false=start disappearing)</param>
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

    /// <summary>
    /// Remap a value from between two value to two new values
    /// </summary>
    /// <param name="value">the value to remap</param>
    /// <param name="from1">the lower bound of the previous range</param>
    /// <param name="from2">the upper bound of the previous range</param>
    /// <param name="to1">the lower bound of the new range</param>
    /// <param name="to2">the upper bound of the new range</param>
    /// <returns>the remapped value</returns>
    float Remap(float value, float from1, float from2, float to1, float to2)
    {
        return to1 + (value - from1) * (to2 - to1) / (from2 - from1);
    }

    /// <summary>
    /// Draws a sphere showing the orbit radius
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (m_orbit && m_orbitTransform)
            Gizmos.DrawWireSphere(m_orbitTransform.position, m_orbitRadius);
    }
}

