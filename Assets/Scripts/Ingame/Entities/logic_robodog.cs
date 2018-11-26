using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class logic_robodog : MonoBehaviour {

    [Header("Robo settings")]
    public float speed = 10;
    public float activeTime = 1;
    public bool activeByDefault = false;
    public LayerMask groundLayer;

    private AudioSource _audio;
    private Rigidbody2D _body;
    private Animator _animator;

    private List<Collider2D> _colliders;
    private List<string> _allowedColliders;

    private float _timer;
    private bool _hasWon;
    private bool _isTimeRunning;
    private bool _active;

    public void Awake () {
        this._body = GetComponent<Rigidbody2D>();
        this._audio = GetComponent<AudioSource>();
        this._animator = GetComponent<Animator>();

        this._colliders = new List<Collider2D>();
        this._allowedColliders = new List<string>() {
            "paradox_object",
            "timed_object"
        };
    }

    public void OnEnable() {
        CoreController.OnTimeChange += this.onTimeChange;
        CoreController.OnGameWin += this.onGameWin;
    }

    public void OnDisable() {
        CoreController.OnTimeChange -= this.onTimeChange;
        CoreController.OnGameWin -= this.onGameWin;
    }

    public void OnTriggerEnter2D(Collider2D collider) {
        if (!this._isTimeRunning) return;
        if (!this._allowedColliders.Contains(collider.tag)) return;
        if (this._colliders.Count > 0 || this._colliders.Contains(collider)) return;

        this._colliders.Add(collider);
        this.activateBot();
    }

    public void OnTriggerExit2D(Collider2D collision) {
        if (!this._colliders.Contains(collision)) return;
        this._colliders.Remove(collision);
    }

    public void Update() {
        if (!this._isTimeRunning || !this._active || Time.time > this._timer) return;
        if (!this.IsGrounded()) return;

        this._body.velocity = new Vector2(transform.localScale.x * speed, _body.velocity.y);
    }

    private void onGameWin() {
        this._isTimeRunning = false;
        this._active = false;
        this._hasWon = true;
    }

    private void onTimeChange(bool started) {
        this._isTimeRunning = started;
        this._active = false;
        this._animator.SetInteger("status", 0);
        this._colliders.Clear();

        if (started && this.activeByDefault) {
            this.activateBot();
        }
    }

    private void activateBot() {
        if (this._active || this._hasWon) return;

        this._active = true;
        this._timer = Time.time + this.activeTime;
        this._audio.Play();

        this._animator.SetInteger("status", 1);
    }

    public bool IsGrounded() {
        return Physics2D.Raycast(transform.position, -Vector3.up, 0.01f, this.groundLayer);
    }
}
