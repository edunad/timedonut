using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class logic_paradoxitem : MonoBehaviour {

    public bool isParadoxItem;

    private Rigidbody2D _body;
    private Camera _camera;
    private SpriteRenderer _sprite;

    // TIME VARS
    private Vector3 _originalPosition;
    private Vector3 _originalAngle;

    // DRAGGING
    private Vector3 _dragOffset;
    private bool _isDragging;

    private Collider2D _collider;

    // TIME
    private bool _timeEnabled;

    public void Awake() {
        this._originalPosition = this.transform.position;
        this._originalAngle = this.transform.eulerAngles;

        this._camera = GameObject.Find("Camera").GetComponent<Camera>();
        this._body = GetComponent<Rigidbody2D>();
        this._sprite = GetComponent<SpriteRenderer>();

        // Disable movement by default
        this._body.bodyType = RigidbodyType2D.Static;

        // SETUP
        this.tag = "time_entity";
        this.gameObject.layer = isParadoxItem ? 11 : 10;
    }

    /* ************* 
     * Core
     ===============*/
    public void Update() {
        if (this._body == null || this._sprite == null) return;
        if (!this.canControlObject()) return;


        if (!this._isDragging) {
            if (this.isMouseOnObject()) this._sprite.color = Color.blue;
            else this._sprite.color = Color.white;
        } else {
            if (this.canPlaceObject()) this._sprite.color = Color.green;
            else this._sprite.color = Color.red;
        }
    }

    /* ************* 
     * Mouse Dragging
     ===============*/
    public void OnMouseDown() {
        if (!this.canControlObject() || !this.isMouseOnObject()) return;

        Vector3 objsPos = this.transform.position;
        this._dragOffset = objsPos - this._camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, objsPos.z));
        this._isDragging = true;

        // Enable movement but freeze rotation //
        this._body.bodyType = RigidbodyType2D.Dynamic;
        this._body.freezeRotation = true;
        this._body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void OnMouseUp() {
        if (!this._isDragging) return;

        this._isDragging = false;
        this._body.freezeRotation = false;
        this._body.collisionDetectionMode = CollisionDetectionMode2D.Discrete;

        if (!this.canPlaceObject()) {
            Debug.Log("Invalid position!");
            this.resetPosition(true);
        }
    }

    public void OnMouseDrag() {
        if (!this.canControlObject() || !this._isDragging) return;

        Vector3 curPosition = this._camera.ScreenToWorldPoint(Input.mousePosition) + this._dragOffset;
        this.transform.position = curPosition;

        // Reset applied physics //
        this._body.velocity = Vector3.zero;
        this._body.angularVelocity = 0;
        this._body.angularDrag = 0;
    }

    private bool isMouseOnObject() {
        RaycastHit2D screenRay = Physics2D.Raycast(this._camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (screenRay.rigidbody == null || screenRay.rigidbody != this._body) return false;
        return true;
    }

    /* ************* 
     * Physics
     ===============*/
    public void enableMovement() {
        this._body.bodyType = RigidbodyType2D.Dynamic;
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (collision == null) return;
        this._collider = collision.collider;
    }

    public void OnCollisionExit2D(Collision2D collision) {
        if (collision == null || this._collider == null) return;

        Collider2D col = collision.collider;
        if (col != this._collider) return;

        this._collider = null;
    }

    private bool canPlaceObject() {
        return this._collider == null;
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

        this._timeEnabled = started;
    }

    private void resetPosition(bool force = false) {
        if (this.isParadoxItem && !force) return;

        this.transform.position = this._originalPosition;
        this.transform.eulerAngles = this._originalAngle;

        this._body.bodyType = RigidbodyType2D.Static;
    }

    private bool canControlObject() {
        return this.isParadoxItem && !this._timeEnabled;
    }
}
