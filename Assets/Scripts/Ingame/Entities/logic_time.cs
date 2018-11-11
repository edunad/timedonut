using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class logic_time : MonoBehaviour {
    private Rigidbody2D _body;

    // TIME VARS
    private Vector3 _originalPosition;
    private Vector3 _originalAngle;

    public void Awake() {
        this._originalPosition = this.transform.position;
        this._originalAngle = this.transform.eulerAngles;
        
        this._body = GetComponent<Rigidbody2D>();
        
        // Disable movement by default
        this._body.bodyType = RigidbodyType2D.Static;

        // SETUP
        this.tag = "timed_object";
        this.gameObject.layer = 10;
    }

    /* ************* 
     * Physics
     ===============*/
    public void enableMovement() {
        this._body.bodyType = RigidbodyType2D.Dynamic;
    }
    
    /* ************* 
     * Time related
     ===============*/

    public void setTimeStatus(bool started) {
        if (started) {
            this.enableMovement();
        } else {
            this.resetPosition();
        }
    }

    private void resetPosition() {
        this.transform.position = this._originalPosition;
        this.transform.eulerAngles = this._originalAngle;
        this._body.bodyType = RigidbodyType2D.Static;
    }
}
