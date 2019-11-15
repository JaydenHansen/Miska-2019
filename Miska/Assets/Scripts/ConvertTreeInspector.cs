#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom inspector for the convert tree script
/// </summary>
[CustomEditor(typeof(ConvertTreesToObject))]
public class ConvertTreeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // adds a button to call the convert function on the script
        ConvertTreesToObject script = (ConvertTreesToObject)target;
        if (GUILayout.Button("Convert"))
        {
            script.Convert();
        }
    }
}
#endif
