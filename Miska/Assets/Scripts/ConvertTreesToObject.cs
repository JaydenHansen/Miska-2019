using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class ConvertTreesToObject : MonoBehaviour
{
    public Transform m_parent;

    public void Convert()
    {
        Terrain terrain = GetComponent<Terrain>();
        TerrainData data = terrain.terrainData;        
        
        foreach(TreeInstance tree in data.treeInstances)
        {
            Vector3 position = new Vector3(tree.position.x * data.size.x, tree.position.y * data.size.y, tree.position.z * data.size.z);

            if (m_parent)
                Instantiate(data.treePrototypes[tree.prototypeIndex].prefab, position, Quaternion.identity, m_parent);
            else
            Instantiate(data.treePrototypes[tree.prototypeIndex].prefab, position, Quaternion.identity);
        }
    }
}
