using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogPat : MonoBehaviour
{
    public Transform m_cameraArm;
    public float m_duration;

    float m_timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = m_cameraArm.forward;

        m_timer += Time.deltaTime;
        if (m_timer >= m_duration)
        {
            gameObject.SetActive(false);
        }
    }

    public void StartPat()
    {
        m_timer = 0;
        gameObject.SetActive(true);
    }
}
