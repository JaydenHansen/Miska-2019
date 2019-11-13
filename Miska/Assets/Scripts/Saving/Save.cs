using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    public SerializableVector3 m_playerPosition;
    public SerializableVector3 m_cameraRotation;
    public int m_playerTrashCount;

    public List<int> m_trashCanTrashLeft = new List<int>();
    public List<List<bool>> m_trashActive = new List<List<bool>>();

}

[System.Serializable]
public class CollectableSave
{
    public bool[] m_collectables;
}

[System.Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public void GetFromVector3(Vector3 old)
    {
        x = old.x;
        y = old.y;
        z = old.z;
    }
    public Vector3 GetVector3()
    {
        return new Vector3(x, y, z);
    }
}
