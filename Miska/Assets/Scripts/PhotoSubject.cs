using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum JournalSubject
{
    INACTIVE,
    CLN_Station,
    CLN_Rocks,
    CLN_Ducks,
    LOC_River,
    OBJ_DogToy
};

[RequireComponent(typeof(Collider))]
public class PhotoSubject : MonoBehaviour
{
    public bool m_isActive;

    Collider m_coll;
    public GameObject m_player;
    bool isPlayerInPosition;


    public Vector3 m_targetAngle;
    public float m_validAngleOffset;
    public RawImage m_poloroid;

    JournalSubject m_subject;

    string filename;

    //FUNCTIONS TO ADD: Load Image, Send Image to Poloroid, Validator,

    // Start is called before the first frame update
    void Start()
    {
        m_coll = GetComponent<Collider>();
        isPlayerInPosition = false;
    }

    public JournalSubject getSubject()
    {
        return m_subject;
    }

    public void SetActiveState(bool status) //Use to turn journal photo modes on and off
    {
        m_isActive = status;
    }

    public bool GetActiveState()
    {
        return m_isActive;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject == m_player)
        {
            isPlayerInPosition = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject == m_player)
        {
            isPlayerInPosition = false;
        }
    }
    
    public bool isPhotoValid()
    {
        if(!m_isActive)
        {
            return false;
        }
        else
        {
            Vector3 playerFacing = m_player.transform.forward;
            bool isPlayerAngleValid = Vector3.Angle(playerFacing, m_targetAngle) < m_validAngleOffset;
            return isPlayerInPosition && isPlayerAngleValid;
        }
    }


    public void SetupPoloroid(string filename) //pass photo as an argument
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filename))
        {
            fileData = File.ReadAllBytes(filename);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        }

        m_poloroid.texture = tex;
    }
}
