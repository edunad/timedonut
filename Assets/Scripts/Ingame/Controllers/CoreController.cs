using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CoreController : MonoBehaviour {
    [Header("Scene timing")]
    public float maxSceneTime;
    public float sceneDeathTime;

    [Header("Controllers")]
    public HUDController HUDController;

    [HideInInspector]
    public AntiParadoxController AntiParaController;

    public delegate void onTimeChange(bool start);
    public static event onTimeChange OnTimeChange;

    [HideInInspector]
    public bool timeRunning;
    [HideInInspector]
    public float currentTime;

    private float _startTime;

    public void Awake() {
        this.AntiParaController = ScriptableObject.CreateInstance<AntiParadoxController>();
        this.HUDController = GameObject.Find("HUD_Camera").GetComponent<HUDController>();

        this.AntiParaController.init();
    }

    public void Update() {
        if (this.timeRunning)
            this.currentTime = Mathf.Clamp(Time.time - this._startTime, 0, this.maxSceneTime);

        util_timer.Update();
    }

    public void OnDestroy() {
        util_timer.Clear();
    }

    public void onTimeClick() {
        this.setTimeStatus(!this.timeRunning);
    }

    public void setTimeStatus(bool start) {
        this.currentTime = 0f;
        this._startTime = Time.time;

        this.timeRunning = start;
        if (OnTimeChange != null) OnTimeChange(start);
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

            CoreController core = (target as CoreController);
            if (GUILayout.Button("Toggle time")) {
                core.setTimeStatus(!core.timeRunning);
            }

            if (GUILayout.Button("Toggle Anti-Paradoxes")) {
                core.AntiParaController.setVisibility(!core.AntiParaController.isVisible);
            }

        EditorGUILayout.EndHorizontal();
    }
}
