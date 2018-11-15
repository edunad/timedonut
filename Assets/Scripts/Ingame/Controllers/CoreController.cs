using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CoreController : MonoBehaviour {
    public static AntiParadoxController AntiParaController;
    public static TimeController TimeController;
    public static HUDController HUDController;

    public void Awake() {
        AntiParaController = ScriptableObject.CreateInstance<AntiParadoxController>();
        TimeController = ScriptableObject.CreateInstance<TimeController>();
        HUDController = GameObject.Find("HUD_Camera").GetComponent<HUDController>();

        AntiParaController.init();
    }

    public void Update() {
        util_timer.Update();
    }

    public void OnDestroy() {
        util_timer.Clear();
    }

    public static void onTimeClick() {
        TimeController.timeStatus(!TimeController.timeRunning);
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
