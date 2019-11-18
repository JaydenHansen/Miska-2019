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

/// <summary>
/// Contains information of photographable journal subjects (incl. Journal subject, photo and text image objects, photo validation)
/// </summary>
[RequireComponent(typeof(Collider))]
public class PhotoSubject : MonoBehaviour
{
    public      JournalSubject  m_subject;
    public      bool            m_isActive;
    public      AK.Wwise.Event  m_journalUpdateSound;

    private     RawImage        m_poloroid;
    private     Image           m_textImage;
    private     GameObject      m_journalEntry;

    [Header("Photo Validation")]
    private     Collider        m_coll;
    public      GameObject      m_targetOBJ;              //Physical object in scene that used to determine direction validity
    public      float           m_validAngleOffset;
    public      GameObject      m_player;
    public      GameObject      m_camera;



    [Header("Visuals")]
    public      Sprite          m_renderedText;                       //Refers to the actual pre-rendered Image
    public      GameObject      m_uiIcon;



    // Start is called before the first frame update
    void Start()
    {
        m_coll = GetComponent<Collider>();
    }

    /// <summary>
    /// Get the subject associated with this object
    /// </summary>
    public JournalSubject getSubject()
    {
        return m_subject;
    }

    /// <summary>
    /// Get the Journal Entry associated with this object
    /// </summary>
    public GameObject getJournalEntry()
    {
        return m_journalEntry;
    }

    /// <summary>
    /// Turns on/off photo subject validation functionality
    /// </summary>
    /// <param name="status"></param>
    public void SetActiveState(bool status)
    {
        m_isActive = status;
    }

    /// <summary>
    /// Gets activation state of subject validation functionality
    /// </summary>
    public bool GetActiveState()
    {
        return m_isActive;
    }
    
    /// <summary>
    /// Checks if photo is valid for use as a journal photo
    /// </summary>
    public bool isPhotoValid() 
    {
        if(!m_isActive)
        {
            return false;
        }
        else
        {
            bool isPlayerInPosition = m_coll.bounds.Contains(m_player.transform.position);  //Checks player is in correct position

            Vector3 playerFacing = m_camera.transform.forward;
            Vector3 disp = m_camera.transform.position - m_targetOBJ.transform.position;
            float ang = CalculateAngle(playerFacing, disp);                                 //Gets angle between players facing direcetion
            bool isPlayerAngleValid = ang < m_validAngleOffset;

            return isPlayerInPosition && isPlayerAngleValid;
        }
    }

    /// <summary>
    /// Calculates angle between two vectors
    /// </summary>
    float CalculateAngle(Vector3 a, Vector3 b)
    {
        a.Normalize();
        b.Normalize();

        float dot = a.x * b.x + a.y * b.y + a.z * b.z;

        return (float)Mathf.Acos(dot);
    }

    /// <summary>
    /// Sets up journal entry objects
    /// </summary>
    /// <param name="filename">name of photo image file</param>
    /// <param name="entry">journal entry object contains photo & text image object</param>
    public void SetupPoloroid(string filename, GameObject entry) 
    {
        if (entry == null) { return; }
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

    IEnumerator JournalUpdateAudio()
    {
        yield return new WaitForSeconds(0.7f);

        m_journalUpdateSound.Post(gameObject);
        yield return null;
    }
}

