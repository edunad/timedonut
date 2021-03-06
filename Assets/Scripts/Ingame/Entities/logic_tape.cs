﻿
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(util_resetable))]
[RequireComponent(typeof(util_material))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class logic_tape : MonoBehaviour {

    private CoreController _core;

    private Collider2D _stickStart;
    private Collider2D _stickEnd;

    private List<string> _enabledStickingTags;
    private List<Collider2D> _colliders;

    private SpriteRenderer _tape;
    private SpriteRenderer _ducktape;

    private GameObject _glue;
    private FixedJoint2D _stickJoint;

    private Rigidbody2D _body;
    private CircleCollider2D _collision;

    private util_drag _drag;
    private util_material _paradoxMaterial;
    private util_resetable _reset;

    private readonly Color _defaultColor = new Color(0.4f, 0.4f, 0.4f);
    private readonly Color _hoverColor = new Color(1f, 1f, 1f);
    private readonly Color _okColor = new Color(0.15f, 0.68f, 0.37f);
    private readonly Color _errorColor = new Color(0.75f, 0.22f, 0.16f);

    private bool _isAttached;
    private bool _isTimeEnabled;

    private bool _hasWon = false;

    public void Awake() {
        this._core = GameObject.Find("Core").GetComponent<CoreController>();

        this._colliders = new List<Collider2D>();
        this._enabledStickingTags = new List<string>() {
            "static_object",
            "paradox_object",
            "timed_object"
        };

        this._tape = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
        this._ducktape = this.GetComponent<SpriteRenderer>();
        this._collision = this.GetComponent<CircleCollider2D>();

        // Setup reset
        this._reset = this.GetComponent<util_resetable>();
        this._reset.saveObject();

        // SETUP BODY
        this._body = GetComponent<Rigidbody2D>();
        this._body.bodyType = RigidbodyType2D.Kinematic;
        this._body.useFullKinematicContacts = true;

        // SETUP DRAG
        this._drag = ScriptableObject.CreateInstance<util_drag>();
        this._drag.setup(this._body);

        // Set material
        this._paradoxMaterial = GetComponent<util_material>();
        this._paradoxMaterial.setMaterial(new Material(Shader.Find("Paradox_outline_shader")));

        // Setup object
        this.tag = "ducktape_object";
        this.gameObject.layer = 11;

        // Create glue for static objects
        this.createTempGlue();
        this.displayTape(false); // Hide tape

        this.setMaterialColor(this._defaultColor);
    }

    private void createTempGlue() {
        // Create glue object
        if (this._glue != null) return;

        this._glue = new GameObject();
        this._glue.name = "STATIC_TAPE_GLUE";
        this._glue.layer = 13; // No collision
        this._glue.tag = "particle_object";

        Rigidbody2D tempBody = this._glue.AddComponent<Rigidbody2D>();
        tempBody.bodyType = RigidbodyType2D.Kinematic;
        tempBody.freezeRotation = true;
    }

    /* ************* 
     * DRAGGING
     ===============*/
    public void onDrag() {
        if (this.canAttachTape()) {
            this.setMaterialColor(this._okColor);
            this.displayTape(true);
        } else {
            this.setMaterialColor(this._errorColor);
            this.displayTape(false);
        }
    }

    public void OnMouseOver() {
        if (this._isTimeEnabled || this._drag.isDragging || this._hasWon) return;
        this.setMaterialColor(this._hoverColor);
    }

    public void OnMouseExit() {
        if (this._isTimeEnabled || this._drag.isDragging || this._hasWon) return;
        this.setMaterialColor(this._defaultColor);
    }

    public void OnMouseDown() {
        if (this._isTimeEnabled || this._hasWon) return;
        if (!this._drag.onMouseDown(this.transform)) return;

        this._body.freezeRotation = true; // Freeze rotation
        this._body.useFullKinematicContacts = true;

        // Trigger paradox visibility
        this._core.setAntiParadoxVisiblity(true);
        this.setTapeAttach(false); // Reset tape
    }

    public void OnMouseDrag() {
        if (this._isTimeEnabled) return;
        if (!this._drag.onMouseDrag(this.transform)) return;
    }

    public void OnMouseUp() {
        if (this._isTimeEnabled) return;
        if (!this._drag.onMouseUp()) return;

        // Trigger paradox visibility
        this._core.setAntiParadoxVisiblity(false);

        // Unfreeze rotation
        this._body.freezeRotation = false;

        // Attach
        if (this.canAttachTape()) {
            this._stickStart = this._colliders[0];
            this._stickEnd = this._colliders[1];

            this.setTapeAttach(true);
        } else {
            this.setMaterialColor(this._defaultColor);
            this._reset.resetObject();
        }
    }

    /* ************* 
     * ATTACHING
     ===============*/
    private bool canAttachTape() {
        if (this._hasWon || this._colliders.Count != 2) return false;
        foreach (Collider2D col in this._colliders) {
            if (!this._enabledStickingTags.Contains(col.tag)) return false;
        }

        // Why would you attach the tape between 2 static objects
        if (this._colliders[0].tag == "static_object" && this._colliders[1].tag == "static_object") return false;
        return true;
    }

    private void setTapeAttach(bool attach) {
        if (this._isTimeEnabled || this._hasWon) return;

        if (attach) {
            this.attachSticker();
            this.disableAttachedDragging(true);
            this._core.onItemMoved(this.gameObject); // Add it to score
        } else {
            // Destroy prev stick joint
            if (this._stickJoint != null)
                GameObject.Destroy(_stickJoint);

            this.disableAttachedDragging(false);
            this._core.removeItemMoved(this.gameObject); // Undo score
        }

        // Attach
        this.transform.parent = attach ? this._stickStart.gameObject.transform : null;
        this._isAttached = attach;

        this.displayTape(attach); // Display / hide tape
    }


    private void attachSticker() {
        if (this._stickStart == null || this._stickEnd == null) return;

        GameObject start = this._stickStart.gameObject;
        GameObject end = this._stickEnd.gameObject;

        Rigidbody2D startBody = start.GetComponent<Rigidbody2D>();
        Rigidbody2D endBody = end.GetComponent<Rigidbody2D>();

        if (startBody == null && endBody == null) return; // Both static, no need for sticker

        if (startBody == null) {
            start = this.setStaticGluePos(this._stickStart.transform.position);
            startBody = start.GetComponent<Rigidbody2D>();
        } else if (endBody == null) {
            end = this.setStaticGluePos(this._stickEnd.transform.position);
            endBody = end.GetComponent<Rigidbody2D>();
        }

        this._stickJoint = end.gameObject.AddComponent<FixedJoint2D>();
        this._stickJoint.autoConfigureConnectedAnchor = true;
        this._stickJoint.connectedBody = startBody;
    }

    // Prevent dragging
    private void disableAttachedDragging(bool disabled) {
        if (this._stickStart == null || this._stickEnd == null) return;

        GameObject start = this._stickStart.gameObject;
        GameObject end = this._stickEnd.gameObject;

        logic_paradox sPara = start.GetComponent<logic_paradox>();
        if (sPara != null) sPara.drag.isDisabled = disabled;

        logic_paradox ePara = end.GetComponent<logic_paradox>();
        if (ePara != null) ePara.drag.isDisabled = disabled;
    }

    private GameObject setStaticGluePos(Vector3 pos) {
        if (this._glue == null) return null;
        this._glue.transform.position = pos;
        return this._glue;
    }

    /* ************* 
     * EVENTS + TIME
     ===============*/
    private void onWin() {
        this._hasWon = true; // Disable the script
        this._drag.isDisabled = true;

        this.setMovement(false);
    }

    public void OnEnable() {
        CoreController.OnGameWin += this.onWin;
        CoreController.OnTimeChange += this.setTimeStatus;
        this._drag.OnDrag += this.onDrag;
    }


    public void OnDisable() {
        CoreController.OnGameWin -= this.onWin;
        CoreController.OnTimeChange -= this.setTimeStatus;
        this._drag.OnDrag -= this.onDrag;
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

    private void setTimeStatus(bool started) {
        if (this._hasWon) return;

        if (started) {
            this.gameObject.layer = !this._isAttached ? 11 : 13;
            this._collision.enabled = !this._isAttached;
            this.setMovement(!this._isAttached);
        } else {
            this.gameObject.layer = 11;
            this._collision.enabled = true;

            if (!this._isAttached) this._reset.resetObject();

            this.setMovement(false);
        }


        this._colliders.Clear();
        this._isTimeEnabled = started;

        this.displayParadox(!started);
        this.setMaterialColor(this._defaultColor);
    }

    /* ************* 
     * PHYSICS
     ===============*/
    public void OnCollisionEnter2D(Collision2D collision) {
        if (this._hasWon) return;

        Collider2D collider = collision.collider;
        if (this._colliders.Contains(collider)) return;
        this._colliders.Add(collider);
    }

    public void OnCollisionExit2D(Collision2D collision) {
        Collider2D collider = collision.collider;
        if (!this._colliders.Contains(collider)) return;
        this._colliders.Remove(collider);
    }

    /* ************* 
     * RENDERING
     ===============*/
    private void displayTape(bool visible) {
        if (this._tape == null || this._ducktape == null) return;

        this._tape.enabled = visible;
        this._ducktape.enabled = !visible;
    }

    private void setMaterialColor(Color color) {
        if (this._paradoxMaterial == null) return;
        this._paradoxMaterial.setMaterialColor("_paradox_color", color);
    }

    private void displayParadox(bool visible) {
        if (this._paradoxMaterial == null) return;
        this._paradoxMaterial.setMaterialFloat("_material_blend", visible ? 1.6f : 20f);
    }
}
