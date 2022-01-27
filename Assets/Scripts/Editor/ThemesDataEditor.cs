#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
 
[CustomEditor (typeof (ScriptableTheme))]
public class ThemesDataEditor : Editor
{
    override public void OnInspectorGUI ()
    {
        DrawDefaultInspector ();

        ScriptableTheme myScript = (ScriptableTheme)target;
        if (GUILayout.Button ("Update Themes"))
        {
            myScript.UpdateAvailableThemes ();
        }
        if (GUILayout.Button("Randomize Curves"))
        {
            myScript.RandomizeCurves();
        }

    }
}
#endif