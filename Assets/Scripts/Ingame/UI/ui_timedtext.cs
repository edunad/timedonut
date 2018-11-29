
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class ui_timedtext : MonoBehaviour {

    [SerializeField]
    public List<string> dialog;
    public float charPerSec = 0.5f;
    public float startDelay = 0f;
    public float readDelay = 0f;

    private TextMesh _text;
    private float _timeDelay;
    private float _charDelay;
    private bool _enabled;
    private int _currentIndex;
    private bool _readingWait;

    private Action _callback;
    private Action _trigger;

	public void Awake () {
        this._text = GetComponent<TextMesh>();
        this._text.text = "";
    }

    public void startText(Action onComplete, Action trigger = null) {
        if (this._enabled) return;

        this._timeDelay = Time.time + startDelay;
        this._charDelay = Time.time + charPerSec + startDelay;

        this._currentIndex = 0;
        this._enabled = true;
        this._readingWait = false;

        this._text.text = "";
        this._callback = onComplete;
        this._trigger = trigger;
    }

    public void Update() {
        if (!this._enabled && Time.time > this._timeDelay) return;

        if (this._readingWait) {
            if (Time.time > this._timeDelay) {
                this._text.text = "";
                this._readingWait = false;
            }

            return;
        }

        string currentTxt = this.dialog[this._currentIndex];
        if (currentTxt.IndexOf("<trigger>") != -1) {
            this.onEndOfSentence();
            if (this._trigger != null) this._trigger();

            return;
        }else if (this._text.text.Length >= currentTxt.Length) {
            this.onEndOfSentence();
            return;
        }

        if (Time.time > this._charDelay) {
            this._text.text += currentTxt[this._text.text.Length];
            this._charDelay = Time.time + this.charPerSec;
        }
    }

    private void onEndOfSentence() {
        this._currentIndex++;

        if (this._currentIndex >= this.dialog.Count) {
            this._enabled = false;

            util_timer.Simple(this.readDelay, () => {
                this._text.text = "";
                if (this._callback != null) this._callback();
            });
        } else {
            this._readingWait = true;
            this._timeDelay = Time.time + readDelay;
        }
    }
}
