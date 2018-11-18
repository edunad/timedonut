using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_button : MonoBehaviour {

    [Header("Target settings")]
    public GameObject targetObject;
    
    private SpriteRenderer _renderer;

    private float _tapCD;
    private bool _isEnabled;

    private readonly Color32 _normalColor = new Color32(180, 180, 180, 255);

    public void Awake() {
        this._renderer = GetComponent<SpriteRenderer>();
        this._renderer.color = this._normalColor;
        this._isEnabled = true;
    }

    public void OnMouseOver() {
        if (this._renderer == null || !this._isEnabled) return;
        this._renderer.color = Color.white;
    }

    public void OnMouseExit() {
        if (this._renderer == null || !this._isEnabled) return;
        this._renderer.color = this._normalColor;
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
}
