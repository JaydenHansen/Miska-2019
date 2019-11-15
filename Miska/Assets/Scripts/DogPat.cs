using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The hand icon on the dog while it's being patted
/// </summary>
public class DogPat : MonoBehaviour
{
    public Transform m_cameraArm;
    public float m_duration;

    float m_timer;

    // Update is called once per frame
    void Update()
    {
        transform.forward = m_cameraArm.forward; // billboard

        // disables itself after the duration is up
        m_timer += Time.deltaTime;
        if (m_timer >= m_duration)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// enables the gameobject and starts hte timer
    /// </summary>
    public void StartPat()
    {
        m_timer = 0;
        gameObject.SetActive(true);
    }
}
