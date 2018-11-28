
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class CoreController : MonoBehaviour {

    [Header("Scene timing")]
    public float maxSceneTime;
    public float sceneDeathTime;

    [Header("Scene settings")]
    public int goldenDonutMoves;
    public AudioClip backgroundMusic;

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

    public delegate void onMovesUpdate(int moves);
    public static event onMovesUpdate OnMovesUpdate;

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

    private AudioSource _audioSource;
    private float _originalVolume;

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
        this._audioSource = GetComponent<AudioSource>();
        this._audioSource.playOnAwake = false;
        this._audioSource.clip = this.backgroundMusic;
        this._audioSource.loop = true;

        this._originalVolume = 0.25f;
        this.updateMusicVolume();
    }

    /* ************* 
     * MUSIC STUFF
     ===============*/
    public void updateMusicVolume() {
        this._audioSource.volume = Mathf.Clamp(OptionsController.musicVolume / 1f * this._originalVolume, 0f, 1f);
    }

    public void stopMusic() {
        if (this.backgroundMusic == null) return;
        this._audioSource.Stop();
    }

    public void playMusic() {
        if (this.backgroundMusic == null) return;
        this._audioSource.Play();
    }

    public void pauseMusic() {
        if (this.backgroundMusic == null) return;
        this._audioSource.Pause();
    }

    public void unPauseMusic() {
        if (this.backgroundMusic == null) return;
        this._audioSource.UnPause();
    }

    /* ************* 
     * TIMELINE
     ===============*/
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
        if (elementID == "UI_RETRY_BTN") {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else if (elementID == "UI_NEXT_BTN" && this.hasWon) {
            this.loadNextLevel(); // Load next level
        } else if (elementID == "UI_QUIT_BTN" && !this.hasWon) {
            this.quitToMainMenu(); // Quit to mainmenu
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
        int rating = CoreController.RatingController.calculateAndRenderRating(this._movedObjects.Count, this.goldenDonutMoves);

        // Save the level rating
        int sceneId = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("lvl-" + sceneId, rating);
        PlayerPrefs.SetInt("lvl-" + (sceneId + 1), 0); // Unlock next level
        Debug.Log("lvl-" + (sceneId + 1));
        PlayerPrefs.Save();

        if (OnGameWin != null) OnGameWin(); // Alert entities
    }

    private void loadNextLevel() {
        int sceneID = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("loading_scene_index", sceneID + 1);
        SceneManager.LoadScene("level-loader", LoadSceneMode.Single);
    }

    private void quitToMainMenu() {
        PlayerPrefs.SetInt("loading_scene_index", -1); // reset
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    /* ************* 
     * Rating
    ===============*/
    public void onItemMoved(GameObject obj) {
        if (this._movedObjects.Contains(obj)) return;
        this._movedObjects.Add(obj); // TODO : Make undo

        if (OnMovesUpdate != null) OnMovesUpdate(this._movedObjects.Count);
    }

    public void removeItemMoved(GameObject obj) {
        if (!this._movedObjects.Contains(obj)) return;
        this._movedObjects.Remove(obj);

        if (OnMovesUpdate != null) OnMovesUpdate(this._movedObjects.Count);
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