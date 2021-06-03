using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Frame
{
    public float dt;
}

public class GraphButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Save File"))
        {
            ButtonPushed();
        }
    }

    protected virtual void ButtonPushed()
    {
        CreateJson(0, "Assets/test.graph");
    }

    protected void CreateJson(object obj, string path)
    {
        string json = JsonUtility.ToJson(obj, true);
        WriteToFile(json, path);
        //Debug.Log("<color=orange> JsonBuild</color>");
    }

    private void WriteToFile(string json, string path)
    {
        File.Delete(path);
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }
    }
}
