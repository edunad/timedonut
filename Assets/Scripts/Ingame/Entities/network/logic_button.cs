using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logic_button : MonoBehaviour {
    public bool isPreasured;
    public GameObject reciever;

    [HideInInspector]
    public bool isPressed;
    
    private List<Collider2D> _colliders;
    private List<string> _allowedColliders;

    private bool _timeRunning;
    private logic_cable _cable;

    public void Awake() {
        if (this.reciever == null) throw new UnityException("logic_button missing a reciever");
        this._cable = this.GetComponent<logic_cable>();

        this._colliders = new List<Collider2D>();
        this._allowedColliders = new List<string>() {
            "paradox_object",
            "timed_object"
        };
    }

    /* ************* 
     * EVENTS + TIME
     ===============*/
    public void OnEnable() {
        TimeController.OnTimeChange += this.setTimeStatus;
    }

    public void OnDisable() {
        TimeController.OnTimeChange -= this.setTimeStatus;
    }

    public void setTimeStatus(bool running) {
        this._colliders.Clear();
        this.setPressed(false);

        this._timeRunning = running;
    }

    public void setPressed(bool pressed) {
        this.isPressed = pressed;
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
