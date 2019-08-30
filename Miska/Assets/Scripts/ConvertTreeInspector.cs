#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ConvertTreesToObject))]
public class ConvertTreeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ConvertTreesToObject script = (ConvertTreesToObject)target;
        if (GUILayout.Button("Convert"))
        {
            script.Convert();
        }
    }
}
#endif
