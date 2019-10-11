using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConsole : MonoBehaviour
{
    public CameraController m_playerCamera;
    public Camera m_gameCamera;
    public float m_openDelay;
    public GameObject m_frogger;
    public GameObject[] m_buttons;
    public float m_indentDistance;
    public float m_indentSpeed;

    bool m_open;
    bool m_opening;
    float m_timer;

    Vector3 m_startPosition;
    Vector3 m_targetPosition;
    Quaternion m_startRotation;
    Quaternion m_targetRotation;

    float m_baseIndent;
    float[] m_indentTimer;

    // Start is called before the first frame update
    void Start()
    {
        m_targetPosition = m_gameCamera.transform.localPosition;
        m_targetRotation = m_gameCamera.transform.localRotation;

        m_indentTimer = new float[m_buttons.Length];
        m_baseIndent = m_buttons[0].transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_opening)
        {
            m_timer += Time.deltaTime;
            m_gameCamera.transform.localPosition = Vector3.Lerp(m_startPosition, m_targetPosition, m_timer / m_openDelay);
            m_gameCamera.transform.localRotation = Quaternion.Lerp(m_startRotation, m_targetRotation, m_timer / m_openDelay);

            if (m_timer >= m_openDelay)
            {
                m_opening = false;
                m_frogger.SetActive(true);
            }
        }

        if (m_open)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Close();
            }

            for (int i = 0; i < m_buttons.Length; i++)
            {
                KeyCode key = new KeyCode();
                if (i == 0)
                    key = KeyCode.RightArrow;
                else if (i == 1)
                    key = KeyCode.DownArrow;
                else if (i == 2)
                    key = KeyCode.LeftArrow;
                else if (i == 3)
                    key = KeyCode.UpArrow;

                if (Input.GetKey(key))
                {
                    m_indentTimer[i] += Time.deltaTime;
                }
                else
                {
                    m_indentTimer[i] -= Time.deltaTime;
                }
                m_indentTimer[i] = Mathf.Clamp(m_indentTimer[i], 0, m_indentSpeed);

                Vector3 pos = m_buttons[i].transform.position;
                pos.y = m_open ? Mathf.Lerp(m_baseIndent, m_baseIndent - m_indentDistance, m_indentTimer[i] / m_indentSpeed) : m_baseIndent;
                m_buttons[i].transform.position = pos;
            }

        }
    }

    public void Open()
    {
        if (!m_open)
        {
            m_playerCamera.m_camera.enabled = false;
            m_playerCamera.enabled = false;

            m_gameCamera.transform.position = m_playerCamera.transform.position;
            m_gameCamera.transform.rotation = m_playerCamera.transform.rotation;

            m_startPosition = m_gameCamera.transform.localPosition;
            m_startRotation = m_gameCamera.transform.localRotation;

            m_gameCamera.enabled = true;

            m_open = true;
            m_opening = true;
            m_timer = 0f;
        }
    }

    public void Close()
    {
        m_gameCamera.enabled = false;

        m_playerCamera.m_camera.enabled = true;
        m_playerCamera.enabled = true;

        m_open = false;

        m_frogger.SetActive(false);
    }
}
