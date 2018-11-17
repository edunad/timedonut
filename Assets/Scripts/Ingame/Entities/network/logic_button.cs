using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class logic_button : MonoBehaviour {
    public bool isPreasured;
    public GameObject reciever;

    [HideInInspector]
    public bool isPressed;
    
    private List<Collider2D> _colliders;
    private List<string> _allowedColliders;

    private bool _timeRunning;
    private logic_cable _cable;

    private AudioSource _audioSource;
    private AudioClip[] _audioClips;

    public void Awake() {
        if (this.reciever == null) throw new UnityException("logic_button missing a reciever");
        this._cable = this.GetComponent<logic_cable>();

        this._colliders = new List<Collider2D>();
        this._allowedColliders = new List<string>() {
            "paradox_object",
            "timed_object"
        };

        this._audioSource = GetComponent<AudioSource>();
        this._audioSource.playOnAwake = false;
        this._audioSource.volume = 0.13f;

        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Button/button_normal"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Button/button_preasure_on"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Button/button_preasure_off")
        };
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

    public void setTimeStatus(bool running) {
        this._colliders.Clear();
        this.setPressed(false, true);

        this._timeRunning = running;
    }

    public void setPressed(bool pressed, bool skipSound = false) {
        if (this.isPressed == pressed) return;
        this.isPressed = pressed;

        if (!skipSound) {
            AudioClip clip = null;
            if (this.isPreasured) {
                clip = pressed ? this._audioClips[1] : this._audioClips[2];
            } else {
                clip = _audioClips[0];
            }

            if(clip != null) {
                this._audioSource.clip = clip;
                this._audioSource.Play();
            }
        }

        if (this._cable != null) this._cable.setCableColor(pressed ? Color.green : Color.red);
    }

    /* ************* 
     * PHYSICS
     ===============*/

    public void OnTriggerEnter2D(Collider2D collider) {
        if (!this._timeRunning || !this._allowedColliders.Contains(collider.tag)) return;
        if (this._colliders.Contains(collider)) return;

        this._colliders.Add(collider);

        this.setPressed(true);
        this.alertLogic();
    }

    public void OnTriggerExit2D(Collider2D collider) {
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
        if (this.reciever == null || !this._timeRunning) return;
        this.reciever.BroadcastMessage("onDataRecieved", new object[]{ this.gameObject, this.isPressed }, SendMessageOptions.DontRequireReceiver);
    }

}
