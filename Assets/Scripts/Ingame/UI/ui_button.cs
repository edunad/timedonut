﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_button : MonoBehaviour {

    [Header("Button settings")]
    public GameObject targetObject;
    public TextMesh _text;

    private float _tapCD;
    private bool _isEnabled;

    private SpriteRenderer _renderer;
    private readonly Color32 _normalColor = new Color32(180, 180, 180, 255);

    public void Awake() {
        if (_text == null) {
            this._renderer = GetComponent<SpriteRenderer>();
        }

        this.setColor(this._normalColor);
        this._isEnabled = true;
    }

    public void OnMouseOver() {
        if (!this._isEnabled) return;
        this.setColor(Color.white);
    }

    public void OnMouseExit() {
        if (!this._isEnabled) return;
        this.setColor(this._normalColor);
    }

    public void OnMouseUp() {
        if (this.targetObject == null || !this._isEnabled) return;
        if (this._tapCD > Time.time - 0.3f) return;

        this._tapCD = Time.time; // Prevent spamming
        this.targetObject.SendMessage("OnUIClick", this.name, SendMessageOptions.DontRequireReceiver);
    }

    public void isEnabled(bool enabled) {
        this._isEnabled = enabled;
    }

    private void setColor(Color cl) {
        if (_text == null) {
            this._renderer.color = cl;
        } else {
            this._text.color = cl;
        }
    }
}
