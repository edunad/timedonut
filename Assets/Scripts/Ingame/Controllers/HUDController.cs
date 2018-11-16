using Kino;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class HUDController : MonoBehaviour {
    // Texture
    [Header("HUD Elements")]
    public Animator timeSprite;
    public GameObject skullObject;
    public GameObject currTimeObject;

    private readonly float WIDTH = 1024f;
    private readonly float HEIGHT = 768f;

    private readonly float MIN_TIME_OFFSET = -0.4f;
    private readonly float MAX_TIME_OFFSET = 0.4f;
   
    private PostProcessVolume _processVolume;
    private AnalogGlitch _glichEffect = null;
    private CoreController _core;

    private util_timer _rewindTimer;

    private float _currentTime;
    private float _deathTimeZone;
    private float _maxTimeZone;

    public void Awake() {
        this._core = GameObject.Find("Core").GetComponent<CoreController>();

        this._processVolume = this.GetComponent<PostProcessVolume>();
        this._processVolume.profile.TryGetSettings(out _glichEffect);
        this._glichEffect.enabled.overrideState = true;

        // Keep aspect ratio
        Camera _hudCamera = this.GetComponent<Camera>();
        _hudCamera.aspect = WIDTH / HEIGHT;

        this._deathTimeZone = this._core.sceneDeathTime;
        this._maxTimeZone = this._core.maxSceneTime;

        this._currentTime = 4;
        this.setSkullPos();

    }

    void OnGUI() {
        //set up scaling
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / WIDTH, Screen.height / HEIGHT, 1));
    }

    private void Update() {
        if (this.skullObject == null) return;

        this._currentTime = Mathf.Clamp(this._core.currentTime, 0, this._maxTimeZone);
        this.setPointerPos();
    }
    /* ************* 
    * Display
    ===============*/
    private void setPointerPos() {
        if (this.currTimeObject == null) return;
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
        pos.x = skullPos;

        this.skullObject.transform.localPosition = pos;
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
    }

    public void OnDisable() {
        CoreController.OnTimeChange -= this.setTimeStatus;
    }

    private void resetBackEffect() {
        this.timeSprite.SetInteger("status", 0); // Done
        this._glichEffect.scanLineJitter.value = 0.03f;
        this._glichEffect.verticalJump.value = 0f;
        this._rewindTimer = null;
    }

    private void setTimeStatus(bool running) {
        if (this.timeSprite == null) return;

        if (!running) {
            this._glichEffect.scanLineJitter.value = 0.4f;
            this._glichEffect.verticalJump.value = 0.03f;
            this._rewindTimer = util_timer.Simple(0.5f, this.resetBackEffect);
        }

        this._glichEffect.enabled.overrideState = !running;
        this.timeSprite.SetInteger("status", running ? 1 : 2);
    }

    public void OnUIClick(string element) {
        if (element != "ui_button_time" || this._rewindTimer != null) return;
        this._core.onTimeClick();
    }

}
