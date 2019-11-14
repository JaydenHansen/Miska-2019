using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public int m_trashCanCount;
    public Transform m_openPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AreaComplete()
    {
        m_trashCanCount--;
        if (m_trashCanCount == 0)
        {
            transform.position = m_openPosition.position;
            transform.rotation = m_openPosition.rotation;
            GameObject.Find("Goal UI").GetComponent<HUD_UI>().SetupReturnToLodge();
        }
    }
}
