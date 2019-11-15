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
        if (m_startTimer <= 0) // if the game has started
        {
            FrogControls();
        }
        else
        {
            // count down before the game starts
            m_startTimer -= Time.deltaTime;
            m_startCountdown.text = Mathf.CeilToInt(m_startTimer).ToString("0");
            if (m_startTimer <= 0)
            {
                m_startCountdown.enabled = false;
            }
        }
    }

    /// <summary>
    /// controls the movement of the frog
    /// </summary>
    void FrogControls()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) && m_gridPos.x + 1 <= 4) // if the player goes right and will not go off the screen
        {
            rb.MovePosition(rb.position + Vector2.right * m_travelDistance); // moves the frog right by the travel distance
            m_gridPos.x++; // moves to next grid pos
            m_step.Post(gameObject); // sound
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && m_gridPos.x - 1 >= 0) // if the player goes left and will not go off the screen
        {
            rb.MovePosition(rb.position + Vector2.left * m_travelDistance);
            m_gridPos.x--;
            m_step.Post(gameObject);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && m_gridPos.y + 1 <= 4) // if the player goes up and will not go off the screen
        {
            rb.MovePosition(rb.position + Vector2.up * m_travelDistance);
            m_gridPos.y++;
            m_step.Post(gameObject);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && m_gridPos.y - 1 >= 0) // if the player goes down and will not go off the screen
        {
            rb.MovePosition(rb.position + Vector2.down * m_travelDistance);
            m_gridPos.y--;
            m_step.Post(gameObject);
        }
    }

    /// <summary>
    /// if the frog collides with the enemy reset the game
    /// </summary>
    /// <param name="col">the other collider in the collision</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Enemy") // if the other is an enemy
        {
            // reset the game
            m_fail.Post(gameObject);
            ResetGame(true);
        }
    }

    /// <summary>
    /// Resets the game
    /// </summary>
    /// <param name="lost">if the player actually lost or just backed out</param>
    public void ResetGame(bool lost)
    {
        m_spawner.ResetGame(); // resets the enemies

        // restart the countdown timer
        m_startTimer = m_startDelay;
        m_startCountdown.enabled = true;

        m_gridPos = new Vector2Int(2, 0); // resets the grid position

        if (lost)
            m_score.CurrentScore = 0; // reset the score

        rb.MovePosition(m_resetPos); // reset the frog position
    }
}
