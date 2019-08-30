using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AreaName
{
    STATION,
    ROCKS,
    DUCKS,
    OPEN_AREA
};


public class ProgressLog : MonoBehaviour
{
    struct AreaProgressData
    {
        public AreaProgressData(int tot)
        {
            totalAreaTrash = tot;
            disposedAreaTrash = 0;
            isAllTrashDisposed = false;
            isPoICompleted = false;
            progress = 0.0f;
        }

        public void addTrash(int i)
        {
            disposedAreaTrash += 1;
            if (disposedAreaTrash < totalAreaTrash)
            {
                progress = (((float)disposedAreaTrash / (float)totalAreaTrash) * 50.0f) + 10.0f;
                Debug.Log("Progress: " + progress.ToString() + ", Disposed: " + disposedAreaTrash.ToString());
            }
            else
            {
                isAllTrashDisposed = true;
                progress = 60.0f;
            }

        }

        public void PoICompleted()
        {
            isPoICompleted = true;
            progress = 90.0f;
        }

        public float getProgress()
        {
            return progress;
        }

        public bool isComplete()
        {
            return isPoICompleted;
        }

        int totalAreaTrash;
        int disposedAreaTrash;
        bool isAllTrashDisposed;
        bool isPoICompleted;
        float progress;
    }


    Dictionary<AreaName, AreaProgressData> m_AreaLookup;

    [Range(0, 10)]
    public int m_rocksTrash, m_stationTrash, m_ducksTrash;

    AreaName m_currentLocation;
    public GameObject m_journal;
    public MusicController m_music;

    // Start is called before the first frame update
    void Start()
    {
        m_AreaLookup = new Dictionary<AreaName, AreaProgressData>();

        m_AreaLookup.Add(AreaName.STATION,      new AreaProgressData(m_stationTrash));
        m_AreaLookup.Add(AreaName.ROCKS,        new AreaProgressData(m_rocksTrash));
        m_AreaLookup.Add(AreaName.DUCKS,        new AreaProgressData(m_ducksTrash));
    }

    public void SetCurrentLocation (AreaName currLoc)
    {
        m_currentLocation = currLoc;
        //Wwise currLoc
        if (currLoc != AreaName.OPEN_AREA)
        {
            //Wwise Set RTPC
        }
    }

    public AreaName GetCurrentLocation ()
    {
        return m_currentLocation;
    }

    public void DisposeTrash (int disp)
    {
        m_AreaLookup[m_currentLocation].addTrash(disp);
        UpdateProgressAudio();
    }

    public void SetPoICompleted()
    {
        m_AreaLookup[m_currentLocation].PoICompleted();
        UpdateProgressAudio();
    }

    public void UpdateProgressAudio()
    {
        float prg = m_AreaLookup[m_currentLocation].getProgress();
      //  m_music.SetProgressTo(m_currentLocation, prg);
    }

    public bool[] getProgressChecks()
    {
        bool[] checklist = new bool[3];

        for (int i = 0; i < 3; i++)
        {
            checklist[i] = m_AreaLookup[(AreaName)i].isComplete();
        }

        return checklist;
    }

}
