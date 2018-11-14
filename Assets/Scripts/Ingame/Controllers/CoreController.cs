using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CoreController : MonoBehaviour {
    public static AntiParadoxController AntiParaController;
    public static TimeController TimeController;

    public void Awake() {
        AntiParaController = ScriptableObject.CreateInstance<AntiParadoxController>();
        TimeController = ScriptableObject.CreateInstance<TimeController>();
        AntiParaController.init();
    }
}


[CustomEditor(typeof(CoreController))]
public class CoreControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (!Application.isPlaying) return;

        /* ************* 
         * DEBUG ZONE
         ===============*/
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug");
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Toggle time")) {
            CoreController.TimeController.timeStatus(!CoreController.TimeController.timeRunning);
        }

        if (GUILayout.Button("Toggle Anti-Paradoxes")) {
            CoreController.AntiParaController.setVisibility(!CoreController.AntiParaController.isVisible);
        }

        EditorGUILayout.EndHorizontal();
    }
}
