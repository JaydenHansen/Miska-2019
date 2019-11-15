using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Frog : MonoBehaviour
{
    public float m_travelDistance;
    public EnemySpawner m_spawner;
    public float m_startDelay;
    public Text m_startCountdown;
    public Score m_score;

    Rigidbody2D rb;
    Vector2 m_resetPos;
    float m_startTimer;
    Vector2Int m_gridPos;

    public AK.Wwise.Event m_step;
    public AK.Wwise.Event m_fail;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        m_resetPos = rb.position;
        m_startTimer = m_startDelay;
        m_gridPos = new Vector2Int(2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_startTimer <= 0)
        {
            FrogControls();
        }
        else
        {
            m_startTimer -= Time.deltaTime;
            m_startCountdown.text = Mathf.CeilToInt(m_startTimer).ToString("0");
            if (m_startTimer <= 0)
            {
                m_startCountdown.enabled = false;
            }
        }
    }

    void FrogControls()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) && m_gridPos.x + 1 <= 4)
        {
            rb.MovePosition(rb.position + Vector2.right * m_travelDistance);
            m_gridPos.x++;
            m_step.Post(gameObject);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && m_gridPos.x - 1 >= 0)
        {
            rb.MovePosition(rb.position + Vector2.left * m_travelDistance);
            m_gridPos.x--;
            m_step.Post(gameObject);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && m_gridPos.y + 1 <= 4)
        {
            rb.MovePosition(rb.position + Vector2.up * m_travelDistance);
            m_gridPos.y++;
            m_step.Post(gameObject);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && m_gridPos.y - 1 >= 0)
        {
            rb.MovePosition(rb.position + Vector2.down * m_travelDistance);
            m_gridPos.y--;
            m_step.Post(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Enemy")
        {
            m_fail.Post(gameObject);
            ResetGame(true);
        }
    }

    public void ResetGame(bool lost)
    {
        m_spawner.ResetGame();
        m_startTimer = m_startDelay;
        m_startCountdown.enabled = true;
        m_gridPos = new Vector2Int(2, 0);

        if (lost)
            m_score.CurrentScore = 0;

        rb.MovePosition(m_resetPos);
    }
}
