using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_fade : MonoBehaviour {

    [Header("Fade settings")]
    public bool fadeIn;
    public float fadeSpeed;
    public float fadeDelay;

    private float _fadeTime;
    private float _timer;
    private SpriteRenderer _sprite;

    public void Awake () {
        this._sprite = GetComponent<SpriteRenderer>();
    }

    public void OnEnable() {
        Color col = this._sprite.color;
        col.a = fadeIn ? 0f : 1f;

        this._fadeTime = Time.time + fadeDelay;
        this._sprite.color = col;
        this._timer = 0;
    }

    public void Update() {
        if (this._sprite == null || Time.time < this._fadeTime) return;
        Color col = this._sprite.color;

        float fade = col.a;
        if(fadeIn) fade = Mathf.Lerp(fade, 1f, this._timer);
        else fade = Mathf.Lerp(fade, 0f, this._timer);

        col.a = fade;

        this._sprite.color = col;
        this._timer += this.fadeSpeed;
    }
}
