using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_button : MonoBehaviour {

    [Header("Target settings")]
    public GameObject TargetObject;
    public Camera UICamera;

    private Camera _uiCamera;
    private SpriteRenderer _renderer;

    private float _tapCD;
    private bool _isEnabled;

    private readonly Color32 _normalColor = new Color32(180, 180, 180, 255);

    public void Awake() {
        if (UICamera != null) this._uiCamera = UICamera;
        else this._uiCamera = GameObject.Find("HUD_Camera").GetComponent<Camera>();

        this._renderer = GetComponent<SpriteRenderer>();
        this._renderer.color = this._normalColor;
    }

    public void OnMouseOver() {
        if (this._renderer == null) return;
        this._renderer.color = Color.white;
    }

    public void OnMouseExit() {
        if (this._renderer == null) return;
        this._renderer.color = this._normalColor;
    }

    public void OnMouseUp() {
        if (TargetObject == null) return;
        if (_tapCD > Time.time - 0.3f) return;

        this._tapCD = Time.time;
        TargetObject.SendMessage("OnUIClick", this.name, SendMessageOptions.DontRequireReceiver);
    }

    public void isEnabled(bool enabled) {
        this._isEnabled = enabled;
    }
}
