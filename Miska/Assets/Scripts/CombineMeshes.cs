using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CombineMeshes : MonoBehaviour
{
    public MeshFilter[] m_subMeshes;
    public bool m_useChildren;

    MeshFilter m_combined;

    public void Combine()
    {
        if (m_useChildren)
        {
            m_subMeshes = GetComponentsInChildren<MeshFilter>();
        }

        Dictionary<Mesh, List<MeshFilter>> meshTypes = new Dictionary<Mesh, List<MeshFilter>>();
        for (int i = 0; i < m_subMeshes.Length; i++)
        {
            if (m_subMeshes[i].gameObject != gameObject)
            {
                if (meshTypes.ContainsKey(m_subMeshes[i].sharedMesh))
                    meshTypes[m_subMeshes[i].sharedMesh].Add(m_subMeshes[i]);
                else
                    meshTypes.Add(m_subMeshes[i].sharedMesh, new List<MeshFilter> { m_subMeshes[i] });
            }

        }

        foreach (var entry in meshTypes)
        {
            List<MeshFilter> sameMeshes = entry.Value;

            int totalVertexCount = sameMeshes[0].sharedMesh.vertexCount * sameMeshes.Count;
            if (totalVertexCount > 65000 || true)
            {
                int meshesPerObject = Mathf.FloorToInt(65000f / sameMeshes[0].sharedMesh.vertexCount);
                int objectCount = Mathf.CeilToInt(totalVertexCount / 65000f);
          
                //CombineInstance[,] combineStorage = new CombineInstance[objectCount, meshesPerObject];
                List<List<CombineInstance>> combineStorage = new List<List<CombineInstance>>();
                for (int i = 0; i < objectCount; i++)
                    combineStorage.Add(new List<CombineInstance>());

                for (int i = 0; i < sameMeshes.Count; i++)
                {
                    if (sameMeshes[i].sharedMesh.subMeshCount == 1)
                    {
                        int index = Mathf.FloorToInt(i / (float)meshesPerObject);
                        CombineInstance newInstance = new CombineInstance();
                        newInstance.subMeshIndex = 0;
                        newInstance.mesh = sameMeshes[i].sharedMesh;
                        newInstance.transform = sameMeshes[i].transform.localToWorldMatrix;
                        combineStorage[index].Add(newInstance);
                        sameMeshes[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        int index = Mathf.FloorToInt(i / (float)meshesPerObject);
                        for (int j = 0; j < sameMeshes[i].sharedMesh.subMeshCount; j++)
                        {
                            CombineInstance newInstance = new CombineInstance();
                            newInstance.subMeshIndex = j;
                            newInstance.mesh = sameMeshes[i].sharedMesh;
                            newInstance.transform = sameMeshes[i].transform.localToWorldMatrix;
                            combineStorage[index].Add(newInstance);
                        }
                        sameMeshes[i].gameObject.SetActive(false);
                    }
                }

                combineStorage.TrimExcess();
                for (int i = 0; i < objectCount; i++)
                {
                    CombineInstance[] temp = combineStorage[i].ToArray();

                    Mesh combinedMesh = new Mesh();
                    combinedMesh.CombineMeshes(temp);
                    combinedMesh.name = sameMeshes[0].sharedMesh.name + " combined " + i.ToString();

                    GameObject newObject = new GameObject(sameMeshes[0].sharedMesh.name + " combined " + i.ToString());
                    newObject.transform.parent = gameObject.transform;

                    MeshFilter newMeshFilter = newObject.AddComponent<MeshFilter>();
                    newMeshFilter.sharedMesh = combinedMesh;

                    newObject.AddComponent<MeshRenderer>().sharedMaterials = sameMeshes[i].GetComponent<Renderer>().sharedMaterials;
                }                
            }
            else
            {
                //m_combined = GetComponent<MeshFilter>();

                //CombineInstance[] combineStorage = new CombineInstance[m_subMeshes.Length];

                //for (int i = 0; i < m_subMeshes.Length; i++)
                //{
                //    combineStorage[i].subMeshIndex = 0;
                //    combineStorage[i].mesh = m_subMeshes[i].sharedMesh;
                //    combineStorage[i].transform = m_subMeshes[i].transform.localToWorldMatrix;
                //    //m_subMeshes[i].gameObject.SetActive(false);
                //}

                //Mesh combinedMesh = new Mesh();
                //combinedMesh.CombineMeshes(combineStorage);
                //combinedMesh.name = "Combined Grass";
                //m_combined.sharedMesh = combinedMesh;
                //gameObject.SetActive(true);
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CombineMeshes))]
public class CombineMeshesInspector : Editor
{
    CombineMeshes m_script;

    private void OnEnable()
    {
        m_script = (CombineMeshes)target;        
    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        m_script.m_useChildren = EditorGUILayout.Toggle("Use Children", m_script.m_useChildren);
        if (!m_script.m_useChildren)
        {
            SerializedProperty subMeshes = serializedObject.FindProperty("m_subMeshes");
            EditorGUILayout.PropertyField(subMeshes, true);
        }
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Combine"))
        {
            m_script.Combine();
        }
    }
}
#endif