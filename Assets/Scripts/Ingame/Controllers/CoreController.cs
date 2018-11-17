using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoreController : MonoBehaviour {
    [Header("Scene timing")]
    public float maxSceneTime;
    public float sceneDeathTime;
    
    // Controllers
    public static HUDController HUDController;
    public static CameraController CameraController;

    public delegate void onAntiParadoxVisibility(bool visible);
    public static event onAntiParadoxVisibility OnAntiParadoxVisibility;

    public delegate void onTimeChange(bool start);
    public static event onTimeChange OnTimeChange;

    public delegate void onGameWin();
    public static event onGameWin OnGameWin;

    public delegate void onGameLosse();
    public static event onGameLosse OnGameLosse;

    [HideInInspector]
    public bool hasLost = false;
    [HideInInspector]
    public bool hasWon = false;

    [HideInInspector]
    public bool timeRunning;
    [HideInInspector]
    public float currentTime;
    [HideInInspector]
    public bool paradoxVisible;

    private float _startTime;

    /* ************* 
     * CORE
    ===============*/
    public void Awake() {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
   
        CoreController.HUDController = GameObject.Find("HUD_Camera").GetComponent<HUDController>();
        CoreController.CameraController = GameObject.Find("Camera").GetComponent<CameraController>();
    }

    public void Update() {
        if (this.timeRunning && !this.hasWon) {
            this.currentTime = Mathf.Clamp(Time.time - this._startTime, 0, this.maxSceneTime);
            if (this.currentTime >= this.maxSceneTime) this.onTimeLimitHit();
        }

        util_timer.Update();
    }

    public void OnDestroy() {
        util_timer.Clear();
    }

    /* ************* 
     * BUTTONS
    ===============*/
    public void OnUIClick(string elementID) {
        if (elementID == "UI_RETRY_BTN") {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else if (elementID == "UI_NEXT_BTN") {

        }
    }

    /* ************* 
     * WIN + LOSSE
    ===============*/
    public void onTargetDeath() {
        if (this.hasWon) return;

        this.hasLost = true;
        this.hasWon = false;
        if (OnGameLosse != null) OnGameLosse();
    }

    private void onTimeLimitHit() {
        if (this.hasWon || this.hasLost) return;

        // Victory!
        this.hasWon = true;
        this.hasLost = false;

        CoreController.CameraController.canControlCamera = false;
        if (OnGameWin != null) OnGameWin();
    }

    /* ************* 
     * TIME
    ===============*/
    public void onTimeClick() {
        this.setTimeStatus(!this.timeRunning);
    }

    private void setTimeStatus(bool start) {
        if (this.hasWon) return;
        this._startTime = Time.time;
        this.timeRunning = start;

        this.hasLost = false;

        if (start) this.setAntiParadoxVisiblity(false);
        if (OnTimeChange != null) OnTimeChange(start);
    }

    /* ************* 
     * OTHER
    ===============*/
    public void setAntiParadoxVisiblity(bool display) {
        this.paradoxVisible = display;
        if (OnAntiParadoxVisibility != null) OnAntiParadoxVisibility(display);
    }

    
}