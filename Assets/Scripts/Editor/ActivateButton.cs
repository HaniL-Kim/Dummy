using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StageButton))]
public class ActivateButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //
        StageButton btn = (StageButton)target;
        //
        if (GUILayout.Button("Activate"))
            btn.Activate();
        if (GUILayout.Button("DeActivate"))
            btn.DeActivate();
    }
}
