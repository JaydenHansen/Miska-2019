using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JournalSubject
{
    CLN_Station,
    CLN_Rocks,
    CLN_Ducks,
    LOC_River,
    OBJ_DogToy,
};

public class PhotoSubject : MonoBehaviour
{
    JournalSubject m_subject;

    RawImage m_journalPoloroid;

    string filename;

    //FUNCTIONS TO ADD: Load Image, Send Image to Poloroid, Validator, collision, 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
