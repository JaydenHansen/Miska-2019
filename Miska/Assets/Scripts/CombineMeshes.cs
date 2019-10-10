﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CombineMeshes : MonoBehaviour
{
    public MeshFilter[] m_subMeshes;
    public bool m_useChildren;
    public bool m_destroySub;
    public bool m_destroyScript;
    public bool m_saveMesh;

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
                    combinedMesh.CombineMeshes(temp, false);
                    
                    List<int[]> subMeshes = new List<int[]>();
                    for (int j = 0; j < combinedMesh.subMeshCount; j++)
                    {
                        subMeshes.Add(combinedMesh.GetTriangles(j));
                    }

                    combinedMesh.subMeshCount = sameMeshes[0].sharedMesh.subMeshCount;

                    List<List<int>> newSubMeshes = new List<List<int>>();
                    for (int j = 0; j < combinedMesh.subMeshCount; j++)
                    {
                        List<int> currentSubMesh = new List<int>();
                        int index = 0;
                        for (int k = 0; k < subMeshes.Count; k++)
                        {
                            if (index == j)
                            {
                                currentSubMesh.AddRange(subMeshes[k]);
                            }

                            if (index++ == combinedMesh.subMeshCount)
                                index = 0;
                        }
                        newSubMeshes.Add(currentSubMesh);
                    }

                    for (int j = 0; j < newSubMeshes.Count; j++)
                    {
                        combinedMesh.SetTriangles(newSubMeshes[j], j);
                    }

                    int[] submesh1 = combinedMesh.GetTriangles(0);
                    int[] submesh2 = combinedMesh.GetTriangles(1);

                    combinedMesh.Optimize();
                    combinedMesh.RecalculateBounds();
                    combinedMesh.RecalculateNormals();

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

    public void CombineIntoParent()
    {
        m_subMeshes = GetComponentsInChildren<MeshFilter>();        
        if (m_subMeshes.Length == 0)
        {
            Debug.Log("No MeshFilters found");
            return;
        }

        List<CombineInstance> combineStorage = new List<CombineInstance>();
        for (int i = 0; i < m_subMeshes.Length; i++)
        {
            CombineInstance newInstance = new CombineInstance();
            newInstance.mesh = m_subMeshes[i].sharedMesh;

            Matrix4x4 matrix = Matrix4x4.identity;
            if (i != 0)
                matrix.SetTRS(m_subMeshes[i].transform.localPosition, m_subMeshes[i].transform.localRotation, m_subMeshes[i].transform.localScale);

            newInstance.transform = matrix;
            combineStorage.Add(newInstance);
            if (m_destroySub && i != 0)
            {
                DestroyImmediate(m_subMeshes[i].gameObject);
            }
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineStorage.ToArray());

        if (m_saveMesh)
        {
            string path = EditorUtility.SaveFilePanel("Save Combined Mesh", "Assets/", "combined", "asset");
            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetProjectRelativePath(path);

            AssetDatabase.CreateAsset(combinedMesh, path);
            AssetDatabase.SaveAssets();                   
        }
        else
        {
            m_subMeshes[0].sharedMesh = combinedMesh;
        }

        if (m_destroyScript)
        {
            DestroyImmediate(this);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CombineMeshes))]
[CanEditMultipleObjects]
public class CombineMeshesInspector : Editor
{
    SerializedObject m_script;

    private void OnEnable()
    {
        m_script = serializedObject;
    }

    public override void OnInspectorGUI()
    {
        SerializedProperty useChildren = m_script.FindProperty("m_useChildren");
        SerializedProperty destroySub = m_script.FindProperty("m_destroySub");
        SerializedProperty destroyScript = m_script.FindProperty("m_destroyScript");
        SerializedProperty saveMesh = m_script.FindProperty("m_saveMesh");

        //DrawDefaultInspector();
        useChildren.boolValue = EditorGUILayout.Toggle("Use Children", useChildren.boolValue);
        destroySub.boolValue = EditorGUILayout.Toggle("Destroy SubMeshes", destroySub.boolValue);
        destroyScript.boolValue = EditorGUILayout.Toggle("Destroy Script", destroyScript.boolValue);
        saveMesh.boolValue = EditorGUILayout.Toggle("Save Mesh", saveMesh.boolValue);        

        if (!useChildren.boolValue)
        {
            SerializedProperty subMeshes = serializedObject.FindProperty("m_subMeshes");
            EditorGUILayout.PropertyField(subMeshes, true);
        }
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Combine"))
        {
            foreach (Object script in m_script.targetObjects)
            {
                ((CombineMeshes)script).Combine();
            }
        }

        if (GUILayout.Button("Combine into parent"))
        {
            foreach (Object script in m_script.targetObjects)
            {
                ((CombineMeshes)script).CombineIntoParent();
            }
        }
    }
}
#endif