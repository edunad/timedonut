using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TimeController : MonoBehaviour {

    public bool timeRunning;
    private logic_paradoxitem[] _timeObjects;

    public void Awake() {
        this._timeObjects = Object.FindObjectsOfType<logic_paradoxitem>();
    }

    public void timeStatus(bool start) {
        if (this._timeObjects.Length <= 0) return;
        for (int i = 0; i < this._timeObjects.Length; i++) {
            if (this._timeObjects[i] == null) continue;
            this._timeObjects[i].setTimeStatus(start);
        }
        this.timeRunning = start;
    }
}

[CustomEditor(typeof(TimeController))]
public class TimeControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TimeController timeControl = (TimeController)target;
        if (GUILayout.Button("Toggle time")) {
            timeControl.timeStatus(!timeControl.timeRunning);
        }
    }
}