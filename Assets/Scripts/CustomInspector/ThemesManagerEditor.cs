#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
 
[CustomEditor (typeof (ThemesManager))]
public class ThemesManagerEditor : Editor
{
    override public void OnInspectorGUI ()
    {
        DrawDefaultInspector ();

        ThemesManager myScript = (ThemesManager)target;
        if (GUILayout.Button ("Update Themes"))
        {
            myScript.UpdateAvailableThemes ();
        }
        
    }
}
#endif