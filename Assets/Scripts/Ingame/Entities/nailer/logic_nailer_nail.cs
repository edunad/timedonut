using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logic_nailer_nail : MonoBehaviour {

    private Rigidbody2D _body;
    private List<Collider2D> _colliders;


    public bool canNail = true;


    public void Awake() {
        this._colliders = new List<Collider2D>();
        this._body = GetComponent<Rigidbody2D>();
    }

    public void OnTriggerExit2D(Collider2D collider) {
        if (!this._colliders.Contains(collider)) return;
        this._colliders.Remove(collider);
    }
    
    public void OnTriggerEnter2D(Collider2D collider) {
        if (!this.canShoot() || !canNail) return;

        logic_rope rope = collider.GetComponentInParent<logic_rope>();
        if (rope == null) return;

        this.canNail = false;

        /* === HACK ZONE */
        this.transform.parent = rope.gameObject.transform;
        Vector3 point = this.transform.localPosition; // There is prob a better way
        this.transform.parent = null;
        /* === HACK ZONE */

        this._body.bodyType = RigidbodyType2D.Static; // DEBUG
        rope.onRopeCut(this.gameObject, collider.gameObject, point);

        if (!this._colliders.Contains(collider)) {
            this._colliders.Add(collider);
        }
    }

    private bool canShoot() {
        return this._colliders.Count <= 0;
    }
}
