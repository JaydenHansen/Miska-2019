using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Frog : MonoBehaviour
{
    public Rigidbody2D rb;

    public float travelDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FrogControls();
    }

    void FrogControls()
    {
            if (Input.GetKeyDown(KeyCode.RightArrow))
                rb.MovePosition(rb.position + Vector2.right * travelDistance);
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                rb.MovePosition(rb.position + Vector2.left * travelDistance);
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                rb.MovePosition(rb.position + Vector2.up * travelDistance);
           else if (Input.GetKeyDown(KeyCode.DownArrow))
                rb.MovePosition(rb.position + Vector2.down * travelDistance);

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Enemy")
        {
            Debug.Log("We Lost");
            Score.currentScore = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
