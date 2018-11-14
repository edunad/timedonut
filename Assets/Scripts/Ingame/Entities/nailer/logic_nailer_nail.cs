using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(logic_particle))]
public class logic_nailer_nail : MonoBehaviour {
    private Rigidbody2D _body;
    private BoxCollider2D[] _collisions;
    private logic_particle _logic;

    private bool isAttached;

    private float spawnTime;
    private const float maxAttachTime = 2; // 1sec

    public void Awake() {
        this._body = GetComponent<Rigidbody2D>();
        this._collisions = GetComponents<BoxCollider2D>();
        this._logic = GetComponent<logic_particle>();

        this.spawnTime = Time.time;
    }

    public void OnTriggerEnter2D(Collider2D collider) {
        if (this.isAttached || Time.time > this.spawnTime + maxAttachTime) return;

        logic_rope_node rope_node = collider.GetComponent<logic_rope_node>();
        if (rope_node == null || rope_node.ropeController == null) return;
        if (collider.tag != "particle_object") return;

        // Temp attachment to get the hit position (locally)
        this.isAttached = true;
        this._logic.canKill = false;

        this.transform.parent = rope_node.transform;
        this._body.bodyType = RigidbodyType2D.Static;

        // Alert the rope
        rope_node.ropeController.onRopeCut(rope_node, this.transform.localPosition, this.transform.position);
        
        /* == DESTROY OBJECTS == */
        GameObject.Destroy(this._body);

        foreach(BoxCollider2D collision in this._collisions)
            GameObject.Destroy(collision);

        /* == FIX POSITION == */
        this.transform.position = Vector3.zero;
        this.transform.localPosition = Vector3.zero;
        this.transform.rotation = Quaternion.Euler(0, 0, 165);
    }
}
