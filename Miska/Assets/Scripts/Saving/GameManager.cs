using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour
{
    public GameObject m_player;
    public CameraController m_camera;
    public TrashHolder m_trashHolder;
    public TrashCan[] m_trashCans;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Save CreateSaveGameObject()
    {
        Save save = new Save();

        save.m_playerPosition.GetFromVector3(m_player.transform.position);
        save.m_cameraRotation.GetFromVector3(m_camera.transform.rotation.eulerAngles);
        save.m_playerTrashCount = m_trashHolder.TrashCount;

        foreach(TrashCan trashCan in m_trashCans)
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

    public void SaveGame()
    {
        Save save = CreateSaveGameObject();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            m_player.GetComponent<CharacterController>().enabled = false;
            m_player.transform.position = save.m_playerPosition.GetVector3();
            m_player.GetComponent<CharacterController>().enabled = true;
            m_camera.SetRotation(save.m_cameraRotation.GetVector3());
            m_player.transform.rotation = Quaternion.Euler(0, save.m_cameraRotation.y, 0);
            m_trashHolder.TrashCount = save.m_playerTrashCount;            

            for (int i = 0; i < m_trashCans.Length && i < save.m_trashCanTrashLeft.Count; i++)
            {
                m_trashCans[i].TrashLeft = save.m_trashCanTrashLeft[i];
                for (int j = 0; j < m_trashCans[i].m_trash.Length; j++)
                {
                    m_trashCans[i].m_trash[j].SetActive(save.m_trashActive[i][j]);
                }
            }
        }
    }
}
