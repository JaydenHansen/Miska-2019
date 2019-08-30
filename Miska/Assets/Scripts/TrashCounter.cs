using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrashCounter : MonoBehaviour
{
    public TrashCan m_trashCan;
    public Text m_text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_text.text = "Trash " + m_trashCan.TrashLeft + "/" + m_trashCan.m_requiredTrash;
    }
}
