using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;



public enum JournalSubject                      //ADD LATER: Cleaned up areas, Objects
{
    LOC_River,
    LOC_DuckPond,
    LOC_Fireflies
}

[RequireComponent(typeof(Collider))]
public class PhotoSubject : MonoBehaviour
{
    public JournalSubject m_subject;
    RawImage m_poloroid;
    Image m_textImage;
    GameObject m_journalEntry;

    public bool m_isActive;
    Collider m_coll;

    public GameObject m_player;
    public GameObject m_camera;

    public GameObject m_targetOBJ;              //Physical object in scene that used to determine direction validity
    public float m_validAngleOffset;

    public Sprite m_renderedText;                       //Refers to the actual pre-rendered Image

    public GameObject m_uiIcon;
                                                // Start is called before the first frame update
    void Start()
    {
        m_coll = GetComponent<Collider>();
    }


    public JournalSubject getSubject()
    {
        return m_subject;
    }

    public GameObject getJournalEntry()
    {
        return m_journalEntry;
    }

    public void SetActiveState(bool status)     //Use to turn journal photo modes on and off
    {
        m_isActive = status;
    }

    public bool GetActiveState()
    {
        return m_isActive;
    }

    //private void OnControllerColliderHit(ControllerColliderHit collision)
    //{
    //    Debug.Log("Collision Detected");
    //    if (collision.collider.gameObject == m_player)
    //    {
    //        isPlayerInPosition = true;
    //        Debug.Log("Player Entered Area.");
    //    }
    //}

    //private void OnTriggerExit(Collision collision)
    //{
    //    if (collision.collider.gameObject == m_player)
    //    {
    //        isPlayerInPosition = false;
    //    }
    //}
    
    public bool isPhotoValid()                          //Checks validity for photo captured for use as a journal photo
    {
        if(!m_isActive)
        {
            return false;
        }
        else
        {
            bool isPlayerInPosition = m_coll.bounds.Contains(m_player.transform.position);

            Vector3 playerFacing = m_camera.transform.forward;
            Vector3 disp = m_camera.transform.position - m_targetOBJ.transform.position;
            float ang = CalculateAngle(playerFacing, disp);
            bool isPlayerAngleValid = ang < m_validAngleOffset;

            return isPlayerInPosition && isPlayerAngleValid;
        }
    }

    float CalculateAngle(Vector3 a, Vector3 b)
    {
        a.Normalize();
        b.Normalize();

        float dot = a.x * b.x + a.y * b.y + a.z * b.z;

        return (float)Mathf.Acos(dot);
    }
    public void SetupPoloroid(string filename, GameObject entry) //passes in photo as an argument then loads as the poloroids texture
    {
        m_journalEntry = entry;
        m_journalEntry.SetActive(true);
        m_poloroid = entry.transform.Find("JournalPhoto").GetComponent<RawImage>();
        m_textImage = entry.transform.Find("Text").GetComponent<Image>();
        

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filename))
        {
            fileData = File.ReadAllBytes(filename);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        }

        m_poloroid.texture = tex;
        m_textImage.sprite = m_renderedText;

        if(m_uiIcon)
        {
            Destroy(m_uiIcon);
        }
        
    }
}

