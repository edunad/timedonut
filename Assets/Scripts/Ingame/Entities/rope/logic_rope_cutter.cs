using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class logic_rope_cutter : MonoBehaviour {

    public logic_rope rope;
    public float cuttingTime = 2f;

    private float _timer;
    private bool _hasCut;
    private bool _enabled;

    private AudioSource _audioSource;
    private float _originalVolume;

    public void Awake () {
        this._audioSource = GetComponent<AudioSource>();
        this._audioSource.playOnAwake = false;
        this._originalVolume = 0.25f;

        this._timer = Time.time + cuttingTime;
        this._hasCut = false;
    }

    public void OnEnable() {
        CoreController.OnTimeChange += onTimeChange;
    }
    
    public void OnDisable() {
        CoreController.OnTimeChange -= onTimeChange;
    }

    private void onTimeChange(bool start) {
        this._hasCut = false;
        this._timer = Time.time + cuttingTime;

        this._enabled = start;
    }

    public void FixedUpdate() {
        if (Time.time < this._timer || this._hasCut || !this._enabled) return;
        
        // Cut the rope
        this._hasCut = true;

        this._audioSource.volume = Mathf.Clamp(OptionsController.effectsVolume / 1f * this._originalVolume, 0f, 1f);
        this._audioSource.Play();
        this.rope.cutRope();
    }
}
