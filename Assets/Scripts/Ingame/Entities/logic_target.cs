using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class logic_target : MonoBehaviour {
	private CoreController _core;
	private BoxCollider2D _boxCollider;

	private bool _timeRunning;
    private bool _isDisabled = false;

	// Use this for initialization
	public void Start () {
		this.name = "logic_target";
		this.gameObject.layer = 12;

		this._boxCollider = GetComponent<BoxCollider2D>();
		this._boxCollider.isTrigger = true;

		this._core = GameObject.Find("Core").GetComponent<CoreController>();
	}

	// Update is called once per frame
	public void OnTriggerEnter2D(Collider2D collision) {
		if (collision == null || !this._timeRunning || this._isDisabled) return;
		Debug.Log("DEATH : <color='red'>" + this._core.currentTime + "</color>"); // For death_time

        this._core.onTargetDeath();
        this._isDisabled = true;
	}

	/* ************* 
	 * EVENTS + TIME
	===============*/
	private void onWin() {
		this._isDisabled = true; // Disable the script
	}

	public void OnEnable() {
		CoreController.OnGameWin += this.onWin;
		CoreController.OnTimeChange += this.setTimeStatus;
	}

	public void OnDisable() {
		CoreController.OnGameWin -= this.onWin;
		CoreController.OnTimeChange -= this.setTimeStatus;
	}

	private void setTimeStatus(bool started) {
		this._timeRunning = started;
        this._isDisabled = false;
	}
}
