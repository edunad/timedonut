
using UnityEngine;

public class logic_rope_node : MonoBehaviour {
    public Rigidbody2D body;
    public HingeJoint2D joint;
    public LineRenderer line;
    public EdgeCollider2D col;

    public logic_rope ropeController;
    public logic_rope_node nextNode;

    public bool isTEMP;

    // RESET STUFF
    private Transform _originalParent;
    private Vector3 _originalPos;
    private Vector3 _originalPosLocal;
    private Quaternion _originalAngle;
    private Quaternion _originalAngleLocal;
    private Vector2 _oldAnchor;
    private Rigidbody2D _oldConnectedBody;
    private RigidbodyType2D _oldBody;
    private logic_rope_node _oldNextNode;
    private float _oldMass;

    public void saveNode() {
        this._originalParent = this.transform.parent;
        this._originalPos = this.transform.position;
        this._originalPosLocal = this.transform.localPosition;
        this._originalAngle = this.transform.rotation;
        this._originalAngleLocal = this.transform.localRotation;

        this._oldNextNode = nextNode;
        this._oldBody = this.body.bodyType;
        this._oldMass = this.body.mass;

        if (this.joint != null) {
            this._oldAnchor = this.joint.anchor;
            this._oldConnectedBody = this.joint.connectedBody;
        }
    }

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

    public void resetNode() {
        this.transform.parent = this._originalParent;
        this.transform.position = this._originalPos;
        this.transform.rotation = this._originalAngle;
        this.transform.localRotation = this._originalAngleLocal;
        this.transform.localPosition = this._originalPosLocal;

        this.nextNode = this._oldNextNode;
        this.body.bodyType = this._oldBody;
        if (this.body.bodyType != RigidbodyType2D.Static) {
            this.body.velocity = Vector3.zero;
            this.body.angularVelocity = 0f;
            this.body.mass = this._oldMass;
        }

        if (this.joint != null) {
            this.joint.anchor = this._oldAnchor;
            this.joint.connectedBody = this._oldConnectedBody;

            this.updateNode();
        }
    }
}