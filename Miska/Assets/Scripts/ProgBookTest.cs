using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgBookTest : MonoBehaviour
{
    public ProgressLog m_progLog;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            AreaName currLoc = m_progLog.GetCurrentLocation();

            if (currLoc == AreaName.STATION)
            {
                m_progLog.SetCurrentLocation(AreaName.ROCKS);
            }
            else if (currLoc == AreaName.ROCKS)
            {
                m_progLog.SetCurrentLocation(AreaName.DUCKS);
            }
            else if (currLoc == AreaName.DUCKS)
            {
                m_progLog.SetCurrentLocation(AreaName.STATION);
            }
            Debug.Log("Current Location set to:" + m_progLog.GetCurrentLocation().ToString());
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            m_progLog.DisposeTrash(1);
        }
    }
}
