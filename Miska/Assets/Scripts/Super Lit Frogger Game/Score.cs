using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text m_scoreText;
    public Text m_highScoreText;

    int m_currentScore = 0;
    public int CurrentScore
    {
        get { return m_currentScore; }
        set
        {
            m_currentScore = value;
            if (m_currentScore > m_highScore) // if the current score is greater than the highscore
                HighScore = m_currentScore; // update the highscore

            m_scoreText.text = m_currentScore.ToString();
        }
    }

    int m_highScore = 0;
    /// <summary>
    /// Changes highscore text
    /// </summary>
    public int HighScore
    {
        get { return m_highScore; }
        set
        {
            m_highScore = value;
            m_highScoreText.text = m_highScore.ToString();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {   
    }    
}
