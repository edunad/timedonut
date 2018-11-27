using Assets.Scripts.models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logic_poison_fan : MonoBehaviour {

    [Header("Poison settings")]
    public float poisonTime = 2f;
    public Color poisonColor = new Color();

    [Header("Sprite")]
    public GameObject spriteObject;
    public Sprite onSprite;
    public Sprite offSprite;

    private float _timer;
    private bool _enabled;
    private bool _timeRunning;
    private float _poison_percent;

    private CoreController _core;
    private logic_target _target;
    private ui_spinning _fanSpinner;
    private ui_counter _counter;

    private SpriteRenderer _poisonHUD;
    private SpriteRenderer _ventRender;

    private AudioSource _audioSource;
    private float _originalVolume;

    public void Awake() {
        this._core = GameObject.Find("Core").GetComponent<CoreController>();
        this._target = GameObject.Find("logic_target").GetComponent<logic_target>();
        this._poisonHUD = GameObject.Find("ui_poison").GetComponent<SpriteRenderer>();

        this._ventRender = this.spriteObject.GetComponent<SpriteRenderer>();

        this._audioSource = GetComponent<AudioSource>();
        this._audioSource.playOnAwake = false;
        this._originalVolume = 0.35f;

        this._counter = GetComponentInChildren<ui_counter>();
        this._fanSpinner = GetComponentInChildren<ui_spinning>();

        this._fanSpinner.spinSpeed = 0;

        this._poisonHUD.color = poisonColor;
        this._ventRender.sprite = this.offSprite;
    }

    public void OnEnable() {
        CoreController.OnTimeChange += onTimeChange;
    }
    
    public void OnDisable() {
        CoreController.OnTimeChange -= onTimeChange;
    }

    public void Update() {
        if (!this._timeRunning || this._core.hasLost) return;

        if (!this._enabled) {
            this._poison_percent = 100 - this.getTimePercentage(this._timer - Time.time);
            this.setPoison(this._poison_percent);

            if (this._poison_percent >= 100) {
                this._target.killPlayer();
                this._enabled = false;
            }

        } else if(this._poison_percent > 0f) {
            this._poison_percent = Mathf.Clamp(this._poison_percent - 2f, 0, 100);
            this.setPoison(this._poison_percent);
        }
    }

    private float getTimePercentage(float val) {
        return ((val - 0) * 100) / (this.poisonTime - 0);
    }

    private void setPoison(float ammount) {
        Color cl = this._poisonHUD.color;
        cl.a = (ammount / 150);

        this._poisonHUD.color = cl;

        this._counter.setPercentage((int)ammount);
    }

    private void onTimeChange(bool start) {
        this._timer = Time.time + this.poisonTime;
        this._timeRunning = start;

        this.setEnabled(false);
        this.setPoison(0);
    }

    private void setEnabled(bool enabled) {
        this._enabled = enabled;

        this._fanSpinner.spinSpeed = enabled ? 800f : 0f;
        this._ventRender.sprite = enabled ? this.onSprite : this.offSprite;

        // Update volume
        this._audioSource.volume = Mathf.Clamp(OptionsController.effectsVolume / 1f * this._originalVolume, 0f, 1f);
        if (enabled) this._audioSource.Play();
    }

    /* ************* 
     * LOGIC
     ===============*/
    public void onDataRecieved(network_data msg) {
        if (msg == null || msg.header != "active") return;
        this.setEnabled(msg.data == 1);
    }
}
