﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// Functionality for the main menu
/// </summary>
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
    public PhotoMode m_photoMode;

    // Start is called before the first frame update
    void Start()
    {
        // disables the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (File.Exists(Application.persistentDataPath + "/collectables.save")) // checks for a collectable save
        {
            // loads the collectable save
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/collectables.save", FileMode.Open);
            CollectableSave save = (CollectableSave)bf.Deserialize(file);
            file.Close();

            for (int i = 0; i < save.m_collectables.Length; i++) // enables the collectables that have been picked up
            {
                if (save.m_collectables[i])
                    m_collectables[i].SetActive(true);
            }
        }

        PlayAnimation(); // plays the starting animation
        m_isLoadingNow = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && m_animation.isPlaying) // skips the starting animation on left click
        {
            m_animation.Stop();
            m_animation.clip.SampleAnimation(m_camera.gameObject, m_animation.clip.length);
        }

        // adds a delay before the book opens
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

    /// <summary>
    /// Coroutine for loading a scene
    /// </summary>
    /// <param name="index">the index to the new scene in the build order</param>
    /// <param name="loadSave">Whether a game should be loaded</param>
    /// <returns>coroutine</returns>
    IEnumerator LoadScene(int index, bool loadSave)
    {
        int oldIndex = SceneManager.GetActiveScene().buildIndex;

        AsyncOperation async = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive); // starts the new scene loading
        while (!async.isDone) // waits for the new scene to load
        {
            yield return null;
        }

        if (loadSave) // if a save should be loaded
        {
            GameObject gameManObject = GameObject.Find("GameManager"); // the gamemanager in the main scene
            if (gameManObject)
            {
                GameManager gameMan = gameManObject.GetComponent<GameManager>();
                if (gameMan)
                {
                    gameMan.LoadGame(); // loads the game
                }
            }
        }        

        AsyncOperation unloadAsync = SceneManager.UnloadSceneAsync(oldIndex); // unloads the menu scene

        while (!unloadAsync.isDone)
            yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(index)); // activates the main scene
    }

    /// <summary>
    /// Loads the main scene and attempts to load a save
    /// </summary>
    public void ContinueGame()
    {
        if (m_isLoadingNow == false)
        {
            StartCoroutine(LoadScene(1, true));
            m_isLoadingNow = true;
        }
    }

    /// <summary>
    /// Starts a new game
    /// </summary>
    public void NewGame()
    {
        // resets the collectable save
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
        m_photoMode.enabled = true;
        m_photoMode.ResetPhotoData();

    }

    /// <summary>
    /// waits for an animation to finish
    /// </summary>
    /// <param name="animation">The animation to wait for</param>
    /// <returns>coroutine</returns>
    private IEnumerator WaitForAnimation(Animation animation)
    {
        do
        {
            yield return null;
        } while (animation.isPlaying);

        m_animationEnd.Invoke();
        m_camera.enabled = true;
    }

    /// <summary>
    /// Plays the starting animation and starts the waiting coroutine
    /// </summary>
    void PlayAnimation()
    {
        m_animation.Play();
        StartCoroutine(WaitForAnimation(m_animation));
    }

    /// <summary>
    /// Starts the delay before opening the book
    /// </summary>
    public void StartBookOpen()
    {
        m_bookOpening = true;
    }
}
