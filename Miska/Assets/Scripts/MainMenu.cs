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
    public Book m_book;
    public float m_bookOpenDelay;

    bool m_isLoadingNow;
    float m_bookOpenTimer;
    bool m_bookOpening;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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

        if (m_bookOpening)
        {
            m_bookOpenTimer += Time.deltaTime;
            if (m_bookOpenTimer > m_bookOpenDelay)
            {
                m_book.enabled = true;
                m_book.Start();
                m_book.OpenBook(0);
                m_bookOpening = false;
            }
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
        CollectableSave save = new CollectableSave();
        save.m_collectables = new bool[m_collectables.Length];        

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/collectables.save");
        bf.Serialize(file, save);
        file.Close();

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

    public void StartBookOpen()
    {
        m_bookOpening = true;
    }
}
