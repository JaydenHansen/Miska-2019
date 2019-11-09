using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public Score m_score;
    public AK.Wwise.Event m_goal;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("You Won");
        m_score.CurrentScore += 100;
        m_goal.Post(gameObject);
        collision.gameObject.GetComponent<Frog>().ResetGame(false);
    }
}
