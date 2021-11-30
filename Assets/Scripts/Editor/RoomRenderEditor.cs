using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
[CustomEditor(typeof(RoomRender))]
public class RoomRenderEditor : Editor
{
    private RoomRender roomRender;
    private void Awake()
    {
        roomRender = (RoomRender) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Populate Room Layout"))
        {

            roomRender.PopulateDataSlots();
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
          
        }
    }
}
