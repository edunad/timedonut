using Kino;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class HUDController : MonoBehaviour {
    // Texture
    [Header("HUD Elements")]
    public Animator timeSprite;
    public Texture skullSprite;

    [Header("Time bar settings")]
    public float hudOffset = 35f;
    public float barOffset = 300f;
    public int maxSubBars = 10;

    private readonly float WIDTH = 1280f;
    private readonly float HEIGHT = 800f;

    private PostProcessVolume _processVolume;
    private AnalogGlitch _glichEffect = null;

    private util_timer _rewindTimer;
    private bool _rewindingTime;

    private Texture2D _backgroundTexture;
    private GUIStyle _textureStyle;
    private List<Vector2> _points;

    private float _end_bar;
    private float _current_time;

    public void Awake() {
        // Setup texture
        this._backgroundTexture = Texture2D.whiteTexture;
        this._textureStyle = new GUIStyle { normal = new GUIStyleState { background = this._backgroundTexture } };

        // Set camera aspect
        this._processVolume = this.GetComponent<PostProcessVolume>();
        this._processVolume.profile.TryGetSettings(out _glichEffect);
        this._glichEffect.scanLineJitter.value = 0.03f;
        this._glichEffect.verticalJump.value = 0f;

        // GENERATE POINTS
        this._points = new List<Vector2>();
        this._end_bar = WIDTH - this.barOffset;

        this._current_time = 40f;
        this.generatePoints();

    }

    private void generatePoints() {
        float prevVal = this._end_bar;

        for (int i = 0; i < this.maxSubBars; i++) {
            float size = Random.Range(10f, 40f);
            float offset = Random.Range(15f, 40f);
            float pos = prevVal + offset;
            prevVal += size + offset;

            this._points.Add(new Vector2(pos, size));
        }
    }

    /* ************* 
     * DRAW
     ===============*/
    public void OnGUI() {
        if (this.skullSprite == null) return;

        //set up scaling
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / WIDTH, Screen.height / HEIGHT, 1));

        // Line group
        GUI.BeginGroup(new Rect(0, 0, WIDTH, HEIGHT));

        // Main bar
        this.drawBar(0, this._end_bar);

        // Draw dashes
        foreach (Vector2 point in this._points)
            this.drawBar(point.x, point.y);

        // Draw time bar
        DrawRectangle(new Rect(this._current_time, HEIGHT - this.hudOffset - 14, 10, 5), Color.black);
        DrawRectangle(new Rect(this._current_time, HEIGHT - this.hudOffset - 9, 10, 25), Color.white);

        // SPRITE

        GUI.DrawTexture(new Rect(this._end_bar - 100, HEIGHT - this.hudOffset - 95, 200, 180), this.skullSprite, ScaleMode.ScaleToFit, true, 0, Color.black, 0f, 0f);
        GUI.DrawTexture(new Rect(this._end_bar - 100, HEIGHT - this.hudOffset - 90, 200, 180), this.skullSprite);
        GUI.EndGroup();
    }

    private void drawBar(float x, float width, float height = 8f) {
        DrawRectangle(new Rect(x, HEIGHT - this.hudOffset - (height - 4), width, (height + 6)), Color.black); // BORDER
        DrawRectangle(new Rect(x, HEIGHT - this.hudOffset, width, height), Color.white);
    }

    private void DrawRectangle(Rect position, Color color) {
        if (this._textureStyle == null) return;
        GUI.backgroundColor = color;
        GUI.Box(position, GUIContent.none, this._textureStyle);
    }
    /* ************* 
     * EVENTS + TIME
     ===============*/
    public void OnEnable() {
        TimeController.OnTimeChange += this.setTimeStatus;
    }

    public void OnDisable() {
        TimeController.OnTimeChange -= this.setTimeStatus;
    }

    private void resetBackEffect() {
        this.timeSprite.SetInteger("status", 0); // Done
        this._glichEffect.scanLineJitter.value = 0.03f;
        this._glichEffect.verticalJump.value = 0f;
        this._rewindingTime = false;
    }

    private void setTimeStatus(bool running) {
        if (this.timeSprite == null) return;

        if (!running) {
            this._glichEffect.scanLineJitter.value = 0.4f;
            this._glichEffect.verticalJump.value = 0.03f;

            this._rewindingTime = true;
            this._rewindTimer = util_timer.Simple(0.5f, this.resetBackEffect);
        }

        this.timeSprite.SetInteger("status", running ? 1 : 2);
    }

    public void OnUIClick(string element) {
        if (element != "ui_button_time" || this._rewindingTime) return;
        CoreController.onTimeClick();
    }

}
