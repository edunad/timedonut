
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(util_resetable))]
public class logic_time : MonoBehaviour {
    private Rigidbody2D _body;
    private util_resetable _reset;
    
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
    public void OnEnable() {
        CoreController.OnTimeChange += this.setTimeStatus;
    }

    public void OnDisable() {
        CoreController.OnTimeChange -= this.setTimeStatus;
    }

    private void setTimeStatus(bool started) {
        this.setMovement(started);
        if (!started) this._reset.resetObject();
    }
}
