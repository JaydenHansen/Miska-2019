using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
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
        Score.currentScore += 100;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
