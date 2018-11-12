using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logic_nailer_nail : MonoBehaviour {
    private Rigidbody2D _body;
    private BoxCollider2D _collision;
    private bool isAttached;

    public void Awake() {
        this._body = GetComponent<Rigidbody2D>();
        this._collision = GetComponent<BoxCollider2D>();
    }

    public void OnTriggerEnter2D(Collider2D collider) {
        if (this._body == null || this._collision == null || this.isAttached) return;

        logic_rope_node rope_node = collider.GetComponent<logic_rope_node>();
        if (rope_node == null || rope_node.ropeController == null) return;

        // Temp attachment to get the hit position (locally)
        this.isAttached = true;

        this.transform.parent = rope_node.transform;
        this._body.bodyType = RigidbodyType2D.Static;

        // Alert the rope
        rope_node.ropeController.onRopeCut(this.gameObject, rope_node, this.transform.position);
        
        /* == ATTACH THE NAIL == */
        GameObject.Destroy(this._body);
        GameObject.Destroy(this._collision);

        this.transform.position = Vector3.zero;
        this.transform.localPosition = Vector3.zero;
        this.transform.rotation = Quaternion.Euler(0, 0, 165);
    }
}
