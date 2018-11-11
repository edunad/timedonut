
using System.Collections.Generic;
using UnityEngine;

public class TimeController : ScriptableObject {

    public bool timeRunning;
    private logic_time[] _timeObjects;
    private logic_paradox[] _paradoxObjects;
    private logic_rope[] _ropeObjects;

    public void init() {
        this._timeObjects = Object.FindObjectsOfType<logic_time>();
        this._paradoxObjects = Object.FindObjectsOfType<logic_paradox>();
        this._ropeObjects = Object.FindObjectsOfType<logic_rope>();
    }

    public void timeStatus(bool start) {
        this.setTimeObjects(start);
        this.setParadoxObjects(start);
        this.setRopeObjects(start);

        this.timeRunning = start;
    }

    private void setTimeObjects(bool start) {
        if (this._timeObjects.Length <= 0) return;
        for (int i = 0; i < this._timeObjects.Length; i++) {
            if (this._timeObjects[i] == null) continue;
            this._timeObjects[i].setTimeStatus(start);
        }
    }

    private void setParadoxObjects(bool start) {
        if (this._paradoxObjects.Length <= 0) return;
        for (int i = 0; i < this._paradoxObjects.Length; i++) {
            if (this._paradoxObjects[i] == null) continue;
            this._paradoxObjects[i].setTimeStatus(start);
        }
    }

    private void setRopeObjects(bool start) {
        if (this._ropeObjects.Length <= 0) return;
        for (int i = 0; i < this._ropeObjects.Length; i++) {
            if (this._ropeObjects[i] == null) continue;
            this._ropeObjects[i].enableMovement(start);
        }
    }
}
