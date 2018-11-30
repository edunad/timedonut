
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


    public delegate void onTrigger(string evt);
    private onTrigger _trigger;

	public void Awake () {
        this._text = GetComponent<TextMesh>();
        this._text.text = "";
    }

    public void stopText() {
        this._text.text = "";
        this.enabled = false;
    }

    public void startText(Action onComplete, onTrigger trigger = null) {
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
            if (this._trigger != null) this._trigger(currentTxt.Replace("<trigger>", ""));

            this.onEndOfSentence(0);
            return;
        } else if (currentTxt.IndexOf("<delay>") != -1) {
            float delay = Convert.ToSingle(currentTxt.Replace("<delay>", ""));
            this.onEndOfSentence(delay);
            return;
        } else if (currentTxt.IndexOf("<loop>") != -1) {
            this._currentIndex = 0;
            this.onEndOfSentence(0);
            return;
        } else if (currentTxt.IndexOf("<speed>") != -1) {
            float newSpeed = Convert.ToSingle(currentTxt.Replace("<speed>", ""));
            this.charPerSec = newSpeed;

            this.onEndOfSentence(0);
            return;
        } else if (this._text.text.Length >= currentTxt.Length) {
            this.onEndOfSentence();
            return;
        }
        
        if (Time.time > this._charDelay) {
            this._text.text += currentTxt[this._text.text.Length];
            this._charDelay = Time.time + this.charPerSec;
        }
    }

    private void onEndOfSentence(float delay = -1) {
        this._currentIndex++;

        if (this._currentIndex >= this.dialog.Count) {
            this._enabled = false;

            if (delay < 0) delay = this.readDelay;
            util_timer.Simple(delay, () => {
                this._text.text = "";
                if (this._callback != null) this._callback();
            });
        } else {
            this._readingWait = true;
            this._timeDelay = Time.time + readDelay;
        }
    }
}
