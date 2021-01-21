using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameChecker : MonoBehaviour
{
    //================================//
    float size = 0.05f;
    float deltaTime = 0.0f;
    GUIStyle style;
    Rect rect;
    //================================//
    private void Start()
    {
        SetGuiStyle();
    }
    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
    //================================//
    private void SetGuiStyle()
    {
        int w = Screen.width, h = Screen.height;
        //
        style = new GUIStyle();
        rect = new Rect(0, 0, w, h * size);
        //
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = (int)((float)h * size);
        style.normal.textColor = Color.white;
    }
    //================================//
    public void OnGUI()
    { // Show FPS
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}
