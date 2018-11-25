using Assets.Scripts.Ingame.Entities.util;
using Kino;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour {
    // Texture
    [Header("HUD Elements")]
    public Animator timeSprite;
    public GameObject skullObject;
    public GameObject currTimeObject;

    public TextMesh currentMovesText;

    [Header("HUD MENU Elements")]
    public GameObject winMenuObject;
    public GameObject winUIPanel;
    public GameObject pauseUIPanel;

    private readonly float WIDTH = 1024f;
    private readonly float HEIGHT = 768f;

    private readonly float MIN_TIME_OFFSET = -0.4f;
    private readonly float MAX_TIME_OFFSET = 0.4f;
   
    private PostProcessVolume _processVolume;
    private AnalogGlitch _glichEffect;
    private Bloom _bloomEffect;

    private CoreController _core;

    private ui_intro _intro;

    private util_timer _rewindTimer;

    private float _currentTime;
    private float _deathTimeZone;
    private float _maxTimeZone;

    private bool _hasWon;
    private bool _isPaused;

    private int _playingIntro;

    private util_variableTransition _bloomTarget;
    private util_variableTransition _tvTarget;

    public void Awake() {
        this._core = GameObject.Find("Core").GetComponent<CoreController>();
        this._intro = this.GetComponentInChildren<ui_intro>();

        this._processVolume = this.GetComponent<PostProcessVolume>();
        this._processVolume.profile.TryGetSettings(out _glichEffect);
        this._processVolume.profile.TryGetSettings(out _bloomEffect);

        // Vars
        this._bloomTarget = new util_variableTransition(40f);
        this._tvTarget = new util_variableTransition(0.86f);

        // Keep aspect ratio
        Camera _hudCamera = this.GetComponent<Camera>();
        _hudCamera.aspect = WIDTH / HEIGHT;

        this._deathTimeZone = this._core.sceneDeathTime;
        this._maxTimeZone = this._core.maxSceneTime;

        this.winMenuObject.SetActive(false);
        this.winUIPanel.SetActive(false);
        this.pauseUIPanel.SetActive(false);

        this.updateTotalMoves(0); // Get the total moves
        this.setSkullPos();
    }

    public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        this.playIntro();
    }

    private void playIntro() {
        if (this._intro == null) return;

        this._bloomEffect.intensity.value = 40f;
        this._intro.gameObject.SetActive(true);

        this._playingIntro = 0;
        this._bloomTarget.transition(2f, 0.28f, () => {
            this._bloomEffect.intensity.value = 2f;
            this._playingIntro++;
        });

        this._tvTarget.transition(0.03f, 0.3f, () => {
            this._glichEffect.scanLineJitter.value = 0.03f;
            this._playingIntro++;
        });

        this._intro.triggerFade(true, new Vector3(1.98f, 0, 1f), new Vector3(-8.5f, 0, 1f), () => {
            this._intro.gameObject.SetActive(false);
        });
    }
    
    private void Update() {
        if (this._hasWon) return;

        // Wait for both effects to finish
        if (this._playingIntro < 2) {
            if (this._bloomEffect != null)
                this._bloomEffect.intensity.value = this._bloomTarget.getVar();
            if (this._glichEffect != null)
                this._glichEffect.scanLineJitter.value = this._tvTarget.getVar();
        } else {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                this.displayPauseMenu(!this._isPaused);
            }
        }

        this._currentTime = Mathf.Clamp(this._core.currentTime, 0, this._maxTimeZone);
        this.setPointerPos();
    }
    

    /* ************* 
    * MENU
    ===============*/
    private void displayPauseMenu(bool display) {
        this._isPaused = display;
        this.pauseUIPanel.SetActive(display);
    }

    private void displayWinMenu() {
        if (this.winMenuObject == null || this._hasWon) return;

        this._hasWon = true;
        this.winMenuObject.SetActive(true);
        this.displayPauseMenu(false);

        util_timer.Simple(0.5f, () => {
            this.winUIPanel.SetActive(true);
        });
    }

    /* ************* 
    * Display GAMEPLAY
    ===============*/
    private void updateTotalMoves(int moves) {
        if (this.currentMovesText == null) return;
        this.currentMovesText.text = moves + " / " + this._core.goldenDonutMoves;
    }

    private void setPointerPos() {
        if (this.currTimeObject == null || this._hasWon) return;
        float posInTime = this.getTimePercentage(this._currentTime);
        float pointerPos = this.invertPercentage(posInTime);

        Vector3 pos = this.currTimeObject.transform.localPosition;
        pos.x = pointerPos;

        this.currTimeObject.transform.localPosition = pos;
    }

    private void setSkullPos() {
        if (this.skullObject == null) return;

        float posInTime = this.getTimePercentage(this._deathTimeZone);
        float skullPos = this.invertPercentage(posInTime);

        Vector3 pos = this.skullObject.transform.localPosition;
        this.skullObject.transform.localPosition = new Vector3(skullPos, pos.y, pos.z);
    }

    private float getPercentage(float val) {
        return ((val - this.MIN_TIME_OFFSET) * 100) / (this.MAX_TIME_OFFSET - this.MIN_TIME_OFFSET);
    }

    private float getTimePercentage(float val) {
        return ((val - 0) * 100) / (this._maxTimeZone - 0);
    }

    private float invertPercentage(float perc) {
        return ((perc * (this.MAX_TIME_OFFSET - this.MIN_TIME_OFFSET) / 100) + this.MIN_TIME_OFFSET);
    }
    

    /* ************* 
    * EVENTS + TIME
    ===============*/
    public void OnEnable() {
        CoreController.OnTimeChange += this.setTimeStatus;
        CoreController.OnGameWin += this.displayWinMenu;
        CoreController.OnGameLosse += this.onGameLosse;
        CoreController.OnMovesUpdate += this.updateTotalMoves;

        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    public void OnDisable() {
        CoreController.OnTimeChange -= this.setTimeStatus;
        CoreController.OnGameWin -= this.displayWinMenu;
        CoreController.OnGameLosse -= this.onGameLosse;
        CoreController.OnMovesUpdate -= this.updateTotalMoves;

        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void onGameLosse() {
        this.setSkullSprite(true);
    }

    private void setSkullSprite(bool died) {
        Animator spr = this.skullObject.GetComponent<Animator>();
        spr.SetInteger("status", died ? 1 : 0);
    }

    private void resetBackEffect() {
        this.timeSprite.SetInteger("status", 0); // Done

        this._glichEffect.scanLineJitter.value = 0.03f;
        this._glichEffect.verticalJump.value = 0f;
        this._rewindTimer = null;
    }

    private void setTimeStatus(bool running) {
        if (this.timeSprite == null || this._hasWon) return;

        if (!running) {
            this._glichEffect.scanLineJitter.value = 0.4f;
            this._glichEffect.verticalJump.value = 0.03f;
            this._rewindTimer = util_timer.Simple(0.5f, this.resetBackEffect);
        }

        this._glichEffect.enabled.overrideState = !running;
        this.timeSprite.SetInteger("status", running ? 1 : 2);
        this.setSkullSprite(false);
    }

    public void OnUIClick(string element) {
        if (this._playingIntro < 2) return;
        if (element != "ui_button_time" || this._rewindTimer != null || this._hasWon) return;
        this._core.toggleTime();
    }
}
