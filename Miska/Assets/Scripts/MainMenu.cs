using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public Animation m_animation;
    public CameraController m_camera;
    public VoidEvent m_animationEnd;
    public GameObject[] m_collectables;

    bool m_isLoadingNow;
    // Start is called before the first frame update
    void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/collectables.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/collectables.save", FileMode.Open);
            CollectableSave save = (CollectableSave)bf.Deserialize(file);
            file.Close();

            for (int i = 0; i < save.m_collectables.Length; i++)
            {
                if (save.m_collectables[i])
                    m_collectables[i].SetActive(true);
            }
        }

        PlayAnimation();
        m_isLoadingNow = false;
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
        {
            yield return null;
        }

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

        AsyncOperation unloadAsync = SceneManager.UnloadSceneAsync(oldIndex);

        while (!unloadAsync.isDone)
            yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(index));
    }

    public void ContinueGame()
    {
        if (m_isLoadingNow == false)
        {
            StartCoroutine(LoadScene(1, true));
            m_isLoadingNow = true;
        }
    }

    public void NewGame()
    {
        if (m_isLoadingNow == false)
        {
            StartCoroutine(LoadScene(1, false));
            m_isLoadingNow = true;
        }
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
