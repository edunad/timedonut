using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CoreController : MonoBehaviour {
    [Header("Scene timing")]
    public float maxSceneTime;
    public float sceneDeathTime;
    
    // Controllers
    public static HUDController HUDController;

    public delegate void onAntiParadoxVisibility(bool visible);
    public static event onAntiParadoxVisibility OnAntiParadoxVisibility;

    public delegate void onTimeChange(bool start);
    public static event onTimeChange OnTimeChange;

    [HideInInspector]
    public bool timeRunning;
    [HideInInspector]
    public float currentTime;
    [HideInInspector]
    public bool paradoxVisible;


    private float _startTime;

    public void Awake() {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
   
        CoreController.HUDController = GameObject.Find("HUD_Camera").GetComponent<HUDController>();
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

    public void setAntiParadoxVisiblity(bool display) {
        this.paradoxVisible = display;
        if (OnAntiParadoxVisibility != null) OnAntiParadoxVisibility(display);
    }

    public void setTimeStatus(bool start) {
        this._startTime = Time.time;
        this.timeRunning = start;

        if (start) this.setAntiParadoxVisiblity(false);
        if (OnTimeChange != null) OnTimeChange(start);
    }
}