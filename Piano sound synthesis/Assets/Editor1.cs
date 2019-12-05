using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Simulator))]
public class Editor1 : Editor
{
    public override void OnInspectorGUI()
    {
        Simulator simulator = (Simulator)target;
        DrawDefaultInspector();
        if (GUILayout.Button("SetUpComputeShader"))
        {
            simulator.SetUpComputeShader();
        }
        if (GUILayout.Button(simulator.recording?"StopRecording": "StartRecording"))
        {
            if(simulator.recording)
                simulator.StopRecording();
            else
                simulator.StartRecording();
        }
    }
}
