﻿using Assets.Scripts.models;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class logic_button : MonoBehaviour {
    [Header("Button settings")]
    public bool isPreasured;

    [Header("Sprite settings")]
    public Sprite offSprite;
    public Sprite onSprite;

    [Header("Networking settings")]
    public GameObject reciever;
    public string dataHeader = "active";

    [HideInInspector]
    public bool isPressed;
    
    private List<Collider2D> _colliders;
    private List<string> _allowedColliders;

    private bool _timeRunning;
    private logic_cable _cable;

    private AudioSource _audioSource;
    private AudioClip[] _audioClips;
    private SpriteRenderer _spriteRender;
    
    private bool _hasWon = false;
    private float _originalVolume;

    public void Awake() {
        if (this.reciever == null) throw new UnityException("logic_button missing a reciever");
        this._cable = this.GetComponent<logic_cable>();

        this._spriteRender = this.GetComponent<SpriteRenderer>();
        this._spriteRender.sprite = this.offSprite;


        this._colliders = new List<Collider2D>();
        this._allowedColliders = new List<string>() {
            "paradox_object",
            "timed_object"
        };

        this._audioSource = GetComponent<AudioSource>();
        this._audioSource.playOnAwake = false;
        this._originalVolume = 0.45f;

        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Button/button_normal"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Button/button_preasure_on"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Button/button_preasure_off")
        };
    }

    /* ************* 
     * EVENTS + TIME
     ===============*/
    private void onWin() {
        this._hasWon = true; // Disable the script
    }

    public void OnEnable() {
        CoreController.OnTimeChange += this.setTimeStatus;
        CoreController.OnGameWin += this.onWin;
    }

    public void OnDisable() {
        CoreController.OnTimeChange -= this.setTimeStatus;
        CoreController.OnGameWin += this.onWin;
    }

    public void setTimeStatus(bool running) {
        if (this._hasWon) return;

        this._colliders.Clear();
        this.setPressed(false, true);

        this._timeRunning = running;
    }

    /* ************* 
     * CORE
     ===============*/
    public void setPressed(bool pressed, bool skipSound = false) {
        if (this.isPressed == pressed || this._hasWon) return;
        this.isPressed = pressed;

        if (!skipSound) {
            AudioClip clip = null;
            if (this.isPreasured) {
                clip = pressed ? this._audioClips[1] : this._audioClips[2];
            } else {
                clip = _audioClips[0];
            }

            if(clip != null) {
                // Update volume
                this._audioSource.volume = Mathf.Clamp(OptionsController.effectsVolume / 1f * this._originalVolume, 0f, 1f);
                this._audioSource.clip = clip;
                this._audioSource.Play();
            }
        }

        if (this._cable != null) this._cable.setCableColor(pressed ? Color.green : Color.red);
        this._spriteRender.sprite = pressed ? this.onSprite : this.offSprite;
    }

    /* ************* 
     * PHYSICS
     ===============*/
    public void OnTriggerEnter2D(Collider2D collider) {
        if (collider == null) return;

        if (this._hasWon) return;
        if (!this._timeRunning || !this._allowedColliders.Contains(collider.tag)) return;
        if (this._colliders.Contains(collider)) return;

        this._colliders.Add(collider);

        this.setPressed(true);
        this.alertLogic();
    }

    public void OnTriggerExit2D(Collider2D collider) {
        if (collider == null) return;

        if (!this._allowedColliders.Contains(collider.tag)) return;
        if (!this._colliders.Contains(collider)) return;
        this._colliders.Remove(collider);

        if (this.isPreasured && this._colliders.Count <= 0) {
            this.setPressed(false);
            this.alertLogic();
        }
    }

    /* ************* 
     * LOGIC
     ===============*/
    private void alertLogic() {
        if (this.reciever == null || !this._timeRunning || this._hasWon) return;
        this.reciever.BroadcastMessage("onDataRecieved",
            new network_data() {
                sender = this.gameObject,
                header = this.dataHeader,
                data = this.isPressed ? 1 : 0
            }, 
            SendMessageOptions.DontRequireReceiver);
    }

}
