using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoSubject : MonoBehaviour
{
    CapsuleCollider m_coll;

    List<GameObject> m_collidedObjects;
   public List<GameObject> m_interestObjects;
    string subjectName;

    // Start is called before the first frame update
    void Start()
    {
        m_coll = this.gameObject.GetComponent<CapsuleCollider>();
        m_coll.enabled = false;
    }

    public string RunIdentifier()
    {
        StartCoroutine("RunSubjectIdentifier");
        return subjectName;
    }

    void OnCollision(Collider collider)
    {
        GameObject go = collider.gameObject;
        m_collidedObjects.Add(go);
    }

    string GetSubjectName()
    {
        foreach (var highlight in m_interestObjects)
        {
            if (CheckForObject(highlight))
            {
                return highlight.name;
            }
        }
        return null;
    }

    bool CheckForObject( GameObject go)
    {
        foreach (var obj in m_collidedObjects)
        {
            if (obj == go)
            {
                return true;
            }
        }
        return false;
    }
    
    IEnumerator RunSubjectIdentifier()
    {
        m_coll.enabled = true;
        yield return new WaitForSeconds(.1f);

         subjectName = GetSubjectName();
    }
}
