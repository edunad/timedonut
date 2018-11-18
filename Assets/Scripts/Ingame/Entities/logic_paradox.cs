
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(util_material))]
[RequireComponent(typeof(util_resetable))]
public class logic_paradox : MonoBehaviour {

    public util_drag drag;

    private Rigidbody2D _body;
    private CoreController _core;

    // DRAGGING
    private List<Collider2D> _colliders;

    // TIME
    private bool _timeEnabled;

    // UTIL
    private util_material _paradoxMaterial;
    private util_resetable _reset;

    private readonly Color _defaultColor = new Color(0.4f, 0.4f, 0.4f);
    private readonly Color _hoverColor = new Color(1f, 1f, 1f);
    private readonly Color _errorColor = new Color(0.75f, 0.22f, 0.16f);

    private bool _hasWon = false;

    public void Awake() {
        this._core = GameObject.Find("Core").GetComponent<CoreController>();

        // Set material
        this._paradoxMaterial = GetComponent<util_material>();
        this._paradoxMaterial.setMaterial(new Material(Shader.Find("Paradox_outline_shader")));

        // Setup RESET
        this._reset = GetComponent<util_resetable>();
        this._reset.saveObject();

        // Set default movement
        this._body = GetComponent<Rigidbody2D>();
        this._body.bodyType = RigidbodyType2D.Kinematic;
        this._body.useFullKinematicContacts = true;

        // Setup drag util
        this.drag = ScriptableObject.CreateInstance<util_drag>();
        this.drag.setup(this._body);

        // SETUP game object
        this.tag = "paradox_object";
        this.gameObject.layer = 11;


        this._colliders = new List<Collider2D>();
    }

    /* ************* 
     * Core
     ===============*/
    public void Update() {
        if (!this.canControlObject() || this._hasWon || !this.drag.isDragging) return;
        
        // ROTATION
        float speed = 15f;
        if (Input.GetKey(KeyCode.LeftShift)) speed = 35f;

        if (Input.GetKey(KeyCode.A)) {
            this.transform.Rotate(new Vector3(0, 0, 8) * Time.deltaTime * speed);
        } else if (Input.GetKey(KeyCode.D)) {
            this.transform.Rotate(new Vector3(0, 0, -8) * Time.deltaTime * speed);
        }
    }

    /* ************* 
     * Mouse Dragging
     ===============*/
    public void onDrag() {
        if (this.canPlaceObject()) {
            this.setMaterialColor(this._hoverColor);
            this.displayGlich(false);
        } else {
            this.setMaterialColor(this._errorColor);
            this.displayGlich(true);
        }
    }

    public void OnMouseOver() {
        if (this.drag.isDragging || this._hasWon) return;
        this.setMaterialColor(this._hoverColor);
    }

    public void OnMouseExit() {
        if (this.drag.isDragging || this._hasWon) return;
        this.setMaterialColor(this._defaultColor);
    }

    public void OnMouseDown() {
        if (!this.canControlObject() || !this.drag.onMouseDown(this.transform) || this._hasWon) return;
        // Freeze rotation //
        this._body.freezeRotation = true;

        // Trigger paradox visibility
        this._core.setAntiParadoxVisiblity(true);
    }

    public void OnMouseUp() {
        if (!this.drag.onMouseUp()) return;

        // Unfreeze rotation //
        this._body.freezeRotation = false;

        // Trigger paradox visibility
        this._core.setAntiParadoxVisiblity(false);

        // Hide Glich
        this.displayGlich(false);

        if (!this.canPlaceObject()) {
            this.resetPosition();
        } else {
            this._reset.saveObject();
        }
    }

    public void OnMouseDrag() {
        if (!this.canControlObject()) return;
        if (!this.drag.onMouseDrag(this.transform)) return;

        this._body.velocity = Vector3.zero; // Reset applied physics
    }

    /* ************* 
     * Sprite
     ===============*/
    private void setMaterialColor(Color color) {
        if (this._paradoxMaterial == null) return;
        this._paradoxMaterial.setMaterialColor("_paradox_color", color);
    }

    private void displayParadox(bool visible) {
        if (this._paradoxMaterial == null) return;
        this._paradoxMaterial.setMaterialFloat("_material_blend", visible ? 0.05f : 0f);
    }

    private void displayGlich(bool visible) {
        if (this._paradoxMaterial == null) return;
        this._paradoxMaterial.setMaterialFloat("_paradox_glich", visible ? 0.25f : 0f);
    }

    /* ************* 
     * Physics
     ===============*/

    public void OnCollisionEnter2D(Collision2D collision) {
        Collider2D col = collision.collider;
        if (this._colliders.Contains(col)) return;
        this._colliders.Add(col);
    }

    public void OnCollisionExit2D(Collision2D collision) {
        Collider2D col = collision.collider;
        if (!this._colliders.Contains(col)) return;
        this._colliders.Remove(col);
    }

    private bool canPlaceObject() {
        return this._colliders.Count <= 0;
    }

    /* ************* 
     * EVENTS + TIME
     ===============*/
    private void onWin() {
        this._hasWon = true; // Disable the script
        this.drag.isDisabled = true;

        this.setMovement(false);
    }

    public void OnEnable() {
        CoreController.OnGameWin += this.onWin;
        CoreController.OnTimeChange += this.setTimeStatus;
        this.drag.OnDrag += this.onDrag;
    }


    public void OnDisable() {
        CoreController.OnGameWin -= this.onWin;
        CoreController.OnTimeChange -= this.setTimeStatus;
        this.drag.OnDrag -= this.onDrag;
    }

    private void setTimeStatus(bool started) {
        if (this._hasWon) return;
        if (!started) this.resetPosition();

        this._timeEnabled = started;

        this.setMovement(started);
        this.displayParadox(!started);
    }

    /* ************* 
     * PHYSICS
     ===============*/
    private void setMovement(bool enabled) {
        this._body.bodyType = enabled ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
        this._body.velocity = Vector3.zero;
        this._body.angularVelocity = 0;
        this._body.freezeRotation = false;
    }
    
    private void resetPosition() {
        this._reset.resetObject();
    }

    private bool canControlObject() {
        return !this._timeEnabled && !this._hasWon;
    }
}
