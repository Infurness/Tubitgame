using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

          
        }
    }
}
