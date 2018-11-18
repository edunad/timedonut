
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(util_resetable))]
public class logic_time : MonoBehaviour {
    private Rigidbody2D _body;
    private util_resetable _reset;

    private bool _hasWon = false;

    public void Awake() {
        this._body = GetComponent<Rigidbody2D>();

        // Setup reset
        this._reset = GetComponent<util_resetable>();
        this._reset.saveObject();

        // Disable movement by default
        this._body.bodyType = RigidbodyType2D.Static;

        // SETUP
        this.tag = "timed_object";
        this.gameObject.layer = 10;
    }

    /* ************* 
     * Physics
     ===============*/
    public void setMovement(bool enabled) {
        this._body.bodyType = enabled ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
    }

    /* ************* 
     * EVENTS + TIME
    ===============*/
    private void onWin() {
        this._hasWon = true; // Disable the script
        this.setMovement(false); // Freeze object
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
        if (this._hasWon) return;

        this._reset.resetObject();
        this.setMovement(started);
    }
}
