using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logic_rope_cutter : MonoBehaviour {

    public logic_rope rope;
    public float cuttingTime = 2f;

    private float _timer;
    private bool _hasCut;
    private bool _enabled;

    public void Awake () {
        this._timer = Time.time + cuttingTime;
        this._hasCut = false;
    }

    public void OnEnable() {
        CoreController.OnTimeChange += onTimeChange;
    }
    
    public void OnDisable() {
        CoreController.OnTimeChange -= onTimeChange;
    }

    private void onTimeChange(bool start) {
        this._hasCut = false;
        this._timer = Time.time + cuttingTime;

        this._enabled = start;
    }

    public void Update() {
        if (Time.time < this._timer || this._hasCut || !this._enabled) return;
        
        // Cut the rope
        this._hasCut = true;
        this.rope.cutRope();
    }
}
