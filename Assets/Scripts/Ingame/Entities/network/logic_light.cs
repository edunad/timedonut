using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logic_light : MonoBehaviour {
    private Animator _animator;

    public void Awake() {
        this._animator = GetComponent<Animator>();
        this._animator.SetInteger("status", 0);
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
        this._animator.SetInteger("status", 0); // Reset
    }

    /* ************* 
     * LOGIC
     ===============*/
    public void onDataRecieved(object[] msg) {
        if (msg == null || msg.Length < 1) return;

        GameObject sender = msg[0] as GameObject;
        if (sender == null) return;
        int data = Convert.ToInt32(msg[1]);

        this._animator.SetInteger("status", data);
    }
}
