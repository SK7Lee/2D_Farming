using UnityEngine;

public class Testing : MonoBehaviour
{
    private float deltaTime = 0.0f;

    void Update()
    {
        // Tính thời gian giữa các khung hình
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        // Hiển thị FPS
        int width = Screen.width, height = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, width, height * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = height * 2 / 50;
        style.normal.textColor = Color.white;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}