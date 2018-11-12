using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logic_rope_node : MonoBehaviour {
    public Rigidbody2D body;
    public HingeJoint2D joint;
    public LineRenderer line;
    public EdgeCollider2D col;

    public logic_rope ropeController;
    public logic_rope_node nextNode;

    public void setRopeController(logic_rope controller) {
        this.ropeController = controller;
    }

    public void updateNode() {
        if (this.line == null || this.col == null || this.joint == null) return;

        this.line.positionCount = 2;
        this.line.SetPosition(0, joint.anchor);
        this.line.SetPosition(1, joint.connectedAnchor);
        this.col.points = new Vector2[] { this.joint.anchor, this.joint.connectedAnchor };
    }
}