using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_button : MonoBehaviour {

    public Vector2 clickArea;
    public GameObject TargetObject;
    public Camera UICamera;

    private float tapCD;
    private Camera _uiCamera;
    private bool _isEnabled;

    void Awake() {
        if (UICamera != null) this._uiCamera = UICamera;
        else this._uiCamera = GameObject.Find("HUD_Camera").GetComponent<Camera>();
    }

    void Update() {
        if (!_isEnabled || Application.platform != RuntimePlatform.Android) return;
        if (TargetObject == null || this._uiCamera == null || Input.touches.Length <= 0)
            return;
        
        foreach (var touch in Input.touches) {
            Vector2 pos = touch.position;
            Vector2 startPos = this._uiCamera.WorldToScreenPoint(this.transform.position);
            Vector2 endPos = this._uiCamera.WorldToScreenPoint(this.transform.position + new Vector3(clickArea.x, clickArea.y, 0f));

            TouchPhase phase = touch.phase;

            if (phase == TouchPhase.Began) {
                tapCD = Time.time;
            } else if (phase == TouchPhase.Ended || phase == TouchPhase.Canceled) {
                if (tapCD > Time.time - 0.3f) {
                    if (pos.x >= startPos.x && pos.x <= endPos.x && pos.y >= startPos.y && pos.y <= endPos.y)
                        TargetObject.SendMessage("OnUIClick", this.name, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }

    public void OnMouseUp() {
        if (TargetObject == null) return;
        if (tapCD > Time.time - 0.3f) return;

        tapCD = Time.time;
        TargetObject.SendMessage("OnUIClick", this.name, SendMessageOptions.DontRequireReceiver);
    }

    public void isEnabled(bool enabled) {
        _isEnabled = enabled;
    }

    void OnDrawGizmos() {
        Vector3 col = transform.position;

        float offsetX = col.x + clickArea.x / 2;
        float offsetY = col.y + clickArea.y / 2;

        // Draw button area
        Gizmos.color = new Color(255, 0, 255, 100);
        Gizmos.DrawWireCube(new Vector3(offsetX, offsetY, col.z), new Vector3(clickArea.x, clickArea.y, col.z + 0.5f));
    }
}
