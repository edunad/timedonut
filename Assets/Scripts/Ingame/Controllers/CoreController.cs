using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoreController : MonoBehaviour {

    [Header("Scene timing")]
    public float maxSceneTime;
    public float sceneDeathTime;

    [Header("Scene settings")]
    public int goldenDonutMoves;

    // Controllers
    public static HUDController HUDController;
    public static CameraController CameraController;
    public static RatingController RatingController;

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
    

    // Private vars
    private float _startTime;
    private List<GameObject> _movedObjects;

    /* ************* 
     * CORE
    ===============*/
    public void Awake() {
        #region Game_Settings
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        #endregion

        #region Controllers
        CoreController.HUDController = GameObject.Find("HUD_Camera").GetComponent<HUDController>();
        CoreController.CameraController = GameObject.Find("Camera").GetComponent<CameraController>();
        CoreController.RatingController = GetComponent<RatingController>();
        #endregion

        this._movedObjects = new List<GameObject>();
    }

    public void Update() {
        #region Game_Timeline
        if (this.timeRunning && !this.hasWon) {
            this.currentTime = Mathf.Clamp(Time.time - this._startTime, 0, this.maxSceneTime);
            if (this.currentTime >= this.maxSceneTime) this.onTimeLimitHit();
        }
        #endregion

        util_timer.Update();
    }

    public void OnDestroy() {
        util_timer.Clear();
    }

    /* ************* 
     * BUTTONS
    ===============*/
    public void OnUIClick(string elementID) {
        if (!this.hasWon) return;

        if (elementID == "UI_RETRY_BTN") {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else if (elementID == "UI_NEXT_BTN") {

        }
    }

    /* ************* 
     * WINNING + LOSING
    ===============*/
    public void onTargetDeath() {
        if (this.hasWon) return; // Already won, how can he losse?

        this.hasLost = true;
        if (OnGameLosse != null) OnGameLosse(); // Alert entities
    }

    private void onTimeLimitHit() {
        if (this.hasWon || this.hasLost) return; // Already won / lost?

        // Victory!
        this.hasWon = true;
        CoreController.CameraController.canControlCamera = false; // Disable camera movement
        CoreController.RatingController.calculateRating(this._movedObjects.Count, this.goldenDonutMoves);

        if (OnGameWin != null) OnGameWin(); // Alert entities
    }

    /* ************* 
     * Rating
    ===============*/
    public void onItemMoved(GameObject obj) {
        if (this._movedObjects.Contains(obj)) return;
        this._movedObjects.Add(obj); // TODO : Make undo
    }

    public void removeItemMoved(GameObject obj) {
        if (!this._movedObjects.Contains(obj)) return;
        this._movedObjects.Remove(obj);
    }
    /* ************* 
     * TIME
    ===============*/
    public void toggleTime() {
        this.setTimeStatus(!this.timeRunning); // Toggle time
    }

    private void setTimeStatus(bool start) {
        if (this.hasWon) return; // Already won, stop toggling time

        this._startTime = Time.time; // Set start time to the current time
        this.currentTime = 0; // Reset

        this.timeRunning = start;
        this.hasLost = false;

        if (start) this.setAntiParadoxVisiblity(false); // Hide paradoxes
        if (OnTimeChange != null) OnTimeChange(start); // Alert entities
    }

    /* ************* 
     * OTHER
    ===============*/
    public void setAntiParadoxVisiblity(bool display) {
        if (this.paradoxVisible == display) return;

        this.paradoxVisible = display;
        if (OnAntiParadoxVisibility != null) OnAntiParadoxVisibility(display); // Alert all paradoxes
    }
}