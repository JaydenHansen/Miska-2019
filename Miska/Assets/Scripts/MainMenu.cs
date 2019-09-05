using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animation m_animation;
    public CameraController m_camera;
    public VoidEvent m_animationEnd;
    // Start is called before the first frame update
    void Start()
    {
        PlayAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && m_animation.isPlaying)
        {
            m_animation.Stop();
            m_animation.clip.SampleAnimation(m_camera.gameObject, m_animation.clip.length);
        }
    }

    IEnumerator LoadScene(int index, bool loadSave)
    {
        int oldIndex = SceneManager.GetActiveScene().buildIndex;

        AsyncOperation async = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

        while (!async.isDone)
            yield return null;

        if (loadSave)
        {
            GameObject gameManObject = GameObject.Find("GameManager");
            if (gameManObject)
            {
                GameManager gameMan = gameManObject.GetComponent<GameManager>();
                if (gameMan)
                {
                    gameMan.LoadGame();
                }
            }

        }

        SceneManager.UnloadSceneAsync(oldIndex);
    }

    public void ContinueGame()
    {
        StartCoroutine(LoadScene(1, true));
    }

    public void NewGame()
    {
        StartCoroutine(LoadScene(1, false));
    }

    private IEnumerator WaitForAnimation(Animation animation)
    {
        do
        {
            yield return null;
        } while (animation.isPlaying);

        m_animationEnd.Invoke();
        m_camera.enabled = true;
    }

    void PlayAnimation()
    {
        m_animation.Play();
        StartCoroutine(WaitForAnimation(m_animation));
    }
}
