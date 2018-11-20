using Assets.Scripts.models;
using System;
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
        CoreController.OnTimeChange += this.setTimeStatus;
    }

    public void OnDisable() {
        CoreController.OnTimeChange -= this.setTimeStatus;
    }

    private void setTimeStatus(bool running) {
        this._animator.SetInteger("status", 0); // Reset
    }

    /* ************* 
     * LOGIC
     ===============*/
    public void onDataRecieved(network_data msg) {
        if (msg == null || msg.header != "active") return;
        this._animator.SetInteger("status", msg.data);
    }
}
