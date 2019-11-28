using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

enum Collectable
{
    Fossil,
    GameBoy,
    Shell
}

/// <summary>
/// Handles all the saving and loading
/// </summary>
public class GameManager : MonoBehaviour
{
    public GameObject m_player;
    public CameraController m_camera;
    public TrashCan[] m_trashCans;
    public bool[] m_collectables;    

    /// <summary>
    /// Creates an object of the save game
    /// </summary>
    /// <returns>the save of the game</returns>
    private Save CreateSaveGameObject()
    {
        Save save = new Save();

        // stores the player position and rotation
        save.m_playerPosition.GetFromVector3(m_player.transform.position);
        save.m_cameraRotation.GetFromVector3(m_camera.transform.rotation.eulerAngles);

        foreach(TrashCan trashCan in m_trashCans) // saves the amount of trash left for each trash can
        {
            save.m_trashCanTrashLeft.Add(trashCan.TrashLeft);
            List<bool> trashActive = new List<bool>();
            foreach (GameObject trash in trashCan.m_trash)
            {
                trashActive.Add(trash.activeSelf);
            }

            save.m_trashActive.Add(trashActive);
        }

        return save;
    }

    /// <summary>
    /// Saves the game to a file
    /// </summary>
    public void SaveGame()
    {
        Save save = CreateSaveGameObject(); // gets the save file

        // saves the file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();
    }

    /// <summary>
    /// loads the saved game
    /// </summary>
    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save")) // if there is a save file
        {
            // load the save
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            // move the player to the saved position
            m_player.GetComponent<CharacterController>().enabled = false;
            m_player.transform.position = save.m_playerPosition.GetVector3();
            m_player.GetComponent<CharacterController>().enabled = true;

            // set the rotation of camera and player
            m_camera.SetRotation(save.m_cameraRotation.GetVector3());
            m_player.transform.rotation = Quaternion.Euler(0, save.m_cameraRotation.y, 0);          

            for (int i = 0; i < m_trashCans.Length && i < save.m_trashCanTrashLeft.Count; i++) // for each trash can
            {
                m_trashCans[i].TrashLeft = save.m_trashCanTrashLeft[i];
                if (m_trashCans[i].TrashLeft == 0) // if the trash can has already been activated
                    m_trashCans[i].m_onAllTrash.Invoke(); // activate the trash can again
                for (int j = 0; j < m_trashCans[i].m_trash.Length; j++)
                {
                    m_trashCans[i].m_trash[j].SetActive(save.m_trashActive[i][j]); // disable trash thats been picked up
                }
            }
        }
    }

    /// <summary>
    /// Save which collectables has been picked up to a file
    /// </summary>
    public void SaveCollectables()
    {
        CollectableSave save = new CollectableSave();

        if (File.Exists(Application.persistentDataPath + "/collectables.save")) // if there is already a collectables save
        {
            // load the save
            BinaryFormatter bfIn = new BinaryFormatter();
            FileStream fileIn = File.Open(Application.persistentDataPath + "/collectables.save", FileMode.Open);
            CollectableSave saveIn = (CollectableSave)bfIn.Deserialize(fileIn);
            fileIn.Close();

            save.m_collectables = new bool[m_collectables.Length];
            // combines the collectables of the save and the current game
            for (int i = 0; i < m_collectables.Length; i++)
            {
                save.m_collectables[i] = m_collectables[i] || saveIn.m_collectables[i];
            }
        }
        else
        {
            save.m_collectables = m_collectables; // save the collectables 
        }       

        // save the collectables to a file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/collectables.save");
        bf.Serialize(file, save);
        file.Close();
    }

    /// <summary>
    /// Load the collectables
    /// </summary>
    public void LoadCollectables()
    {
        if (File.Exists(Application.persistentDataPath + "/collectables.save")) // if there is a collectable save
        {
            // load the file
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/collectables.save", FileMode.Open);
            CollectableSave save = (CollectableSave)bf.Deserialize(file);
            file.Close();

            m_collectables = save.m_collectables; // load the collectables
        }
    }

    /// <summary>
    /// sets the picked up value of the collectable
    /// </summary>
    /// <param name="collectable">the index of the collectable</param>
    public void OnCollectablePickup(int collectable)
    {
        m_collectables[collectable] = true;
        SaveCollectables(); // autosaves the collectables when they are picked up
    }

    /// <summary>
    /// Changes the scene to the menu scene
    /// </summary>
    public void LoadMenuScene()
    {
        StartCoroutine(LoadMenuAsync());
    }

    public IEnumerator LoadMenuAsync()
    {
        int oldIndex = SceneManager.GetActiveScene().buildIndex;

        AsyncOperation async = SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);
        while (!async.isDone)
        {
            yield return null;
        }

        GameObject bookObject = GameObject.Find("Book_menu");
        if (bookObject)
        {
            MainMenu mainMenu = bookObject.GetComponent<MainMenu>();
            if (mainMenu)
            {
                mainMenu.GameFinished();
            }
        }

        SceneManager.UnloadSceneAsync(oldIndex);
    }
}
