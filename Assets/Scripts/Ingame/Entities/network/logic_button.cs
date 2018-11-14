using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logic_button : MonoBehaviour {
    public bool isPreasured;
    public GameObject reciever;

    [HideInInspector]
    public bool isPressed;
    
    private List<Collider2D> _colliders;
    private bool _timeRunning;

    public void Awake() {
        this._colliders = new List<Collider2D>();
        if (this.reciever == null) throw new UnityException("logic_button missing a reciever");
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
        if (!running) {
            this._colliders.Clear();
            this.isPressed = false;
        }

        this._timeRunning = running;
    }

    /* ************* 
     * PHYSICS
     ===============*/

    public void OnTriggerEnter2D(Collider2D collider) {
        if (!this._timeRunning) return;
        if (this._colliders.Contains(collider)) return;

        this._colliders.Add(collider);

        this.isPressed = true;
        this.alertLogic();
    }

    public void OnTriggerExit2D(Collider2D collider) {
        if (!this._colliders.Contains(collider)) return;
        this._colliders.Remove(collider);

        if (this.isPreasured && this._colliders.Count <= 0) {
            this.isPressed = false;
            this.alertLogic();
        }
    }

    /* ************* 
     * LOGIC
     ===============*/
    private void alertLogic() {
        if (this.reciever == null && this._timeRunning) return;
        this.reciever.BroadcastMessage("onDataRecieved", new object[]{ this.gameObject, this.isPressed }, SendMessageOptions.DontRequireReceiver);
    }

}
