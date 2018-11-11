
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class logic_paradox : MonoBehaviour {
    private Rigidbody2D _body;
    private Camera _camera;

    // TIME VARS
    private Vector3 _originalPosition;
    private Vector3 _originalAngle;

    // DRAGGING
    private Vector3 _dragOffset;
    private bool _isDragging;

    private List<Collider2D> _colliders;

    // TIME
    private bool _timeEnabled;

    // EFFECT
    private Material _paradoxMaterial;

    public void Awake() {
        this._originalPosition = this.transform.position;
        this._originalAngle = this.transform.eulerAngles;

        this._camera = GameObject.Find("Camera").GetComponent<Camera>();
        this._body = GetComponent<Rigidbody2D>();

        this._colliders = new List<Collider2D>();

        // Disable movement by default
        this._body.bodyType = RigidbodyType2D.Static;

        // SETUP
        this.tag = "paradox_object";
        this.gameObject.layer = 11;

        // Set material
        this.setParadoxMaterial();
    }

    /* ************* 
     * Core
     ===============*/
    public void Update() {
        if (this._body == null || !this.canControlObject()) return;

        if (!this._isDragging) {
            if (this.isMouseOnObject()) {
                this.setMaterialColor(Color.green);
            } else {
                this.setMaterialColor(Color.white);
            }

        } else {
            if (this.canPlaceObject()) {
                this.setMaterialColor(Color.green);
                this.displayGlich(false);
            } else {
                this.setMaterialColor(Color.red);
                this.displayGlich(true);
            }
        }
    }

    /* ************* 
     * Mouse Dragging
     ===============*/
    public void OnMouseDown() {
        if (!this.canControlObject() || !this.isMouseOnObject()) return;

        // Set draggin start
        Vector3 objsPos = this.transform.position;
        this._dragOffset = objsPos - this._camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, objsPos.z));

        this._isDragging = true;

        // Enable movement but freeze rotation //
        this._body.bodyType = RigidbodyType2D.Kinematic;
        this._body.freezeRotation = true;
        this._body.useFullKinematicContacts = true;
        this._body.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Improve detection

        // Trigger paradox visibility
        CoreController.AntiParaController.setVisibility(true);
    }

    public void OnMouseUp() {
        if (!this._isDragging) return;
        this._isDragging = false;

        this._body.freezeRotation = false;
        this._body.bodyType = RigidbodyType2D.Dynamic;

        // Trigger paradox visibility
        CoreController.AntiParaController.setVisibility(false);

        // Hide Glich
        this.displayGlich(false);

        if (!this.canPlaceObject()) this.resetPosition();
    }

    public void OnMouseDrag() {
        if (!this.canControlObject() || !this._isDragging) return;

        Vector3 curPosition = this._camera.ScreenToWorldPoint(Input.mousePosition) + this._dragOffset;
        this.transform.position = curPosition;

        // Reset applied physics //
        this._body.velocity = Vector3.zero;
    }


    private bool isMouseOnObject() {
        Vector3 mousePos = Input.mousePosition - new Vector3(1f, 1f, 0);
        RaycastHit2D screenRay = Physics2D.Raycast(this._camera.ScreenToWorldPoint(mousePos), Vector2.zero);
        if (screenRay.rigidbody == null || screenRay.rigidbody != this._body) return false;

        return true;
    }

    /* ************* 
     * Sprite
     ===============*/
    private void setMaterialColor(Color color) {
        if (this._paradoxMaterial == null) return;
        this._paradoxMaterial.SetColor("_paradox_color", color);
    }

    private void displayParadox(bool visible) {
        if (this._paradoxMaterial == null) return;
        this._paradoxMaterial.SetFloat("_material_blend", visible ? 0.05f : 0f);
    }

    private void displayGlich(bool visible) {
        if (this._paradoxMaterial == null) return;
        this._paradoxMaterial.SetFloat("_paradox_glich", visible ? 0.25f : 0f);
    }

    private SpriteRenderer[] getRenderSprites() {
        return this.GetComponentsInChildren<SpriteRenderer>();
    }

    private void setParadoxMaterial() {
        _paradoxMaterial = new Material(Shader.Find("Paradox_outline"));

        SpriteRenderer[] renderers = this.getRenderSprites();
        if (renderers == null || renderers.Length <= 0) return;

        foreach (SpriteRenderer spr in renderers) {
            if (spr == null) continue;
            spr.material = this._paradoxMaterial;
        }
    }


    /* ************* 
     * Physics
     ===============*/
    public void enableMovement() {
        this._body.bodyType = RigidbodyType2D.Dynamic;
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        Collider2D col = collision.collider;
        if (col.tag.IndexOf("paradox_object") != -1) return; // Ignore other paradox items

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
     * Time related
     ===============*/

    public void setTimeStatus(bool started) {
        if (started) {
            this.enableMovement();
            this.saveParadoxPosition();
        } else {
            this.resetPosition();
        }

        this._timeEnabled = started;
        this.displayParadox(!started);
    }

    private void resetPosition() {
        this.transform.position = this._originalPosition;
        this.transform.eulerAngles = this._originalAngle;
    }

    private void saveParadoxPosition() {
        this._originalPosition = this.transform.position;
        this._originalAngle = this.transform.eulerAngles;
    }


    private bool canControlObject() {
        return !this._timeEnabled;
    }
}
