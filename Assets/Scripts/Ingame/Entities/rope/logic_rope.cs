using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class logic_rope : MonoBehaviour {
    public GameObject end;

    public Vector3 ropeOffset;
    public float ropeWidth = 0.05f;
    public float ropeNodeLength = 0.6f;

    public Material ropeMaterial;

    // END NODE
    private logic_rope_node _endNode;
    private Rigidbody2D _endBody;

    // TIME VARS
    private Vector3 _end_originalPosition;
    private Vector3 _end_originalLocalPosition;
    private Quaternion _end_originalAngle;
    private bool _timeRunning;

    // LIST
    private List<logic_rope_node> _ropeNodes;
   
    public void Awake() {
        this._ropeNodes = new List<logic_rope_node>();
        this._endBody = this.end.GetComponent<Rigidbody2D>();

        this._endNode = this.end.AddComponent<logic_rope_node>();
        this._endNode.setRopeController(this);

        // Store original pos
        this._end_originalPosition = this.end.transform.position;
        this._end_originalLocalPosition = this.end.transform.localPosition;
        this._end_originalAngle = this.end.transform.rotation;

        this.generateRope();
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

    private void setTimeStatus(bool running) {
        for (int i = 1; i < this._ropeNodes.Count - 1; i++) {
            logic_rope_node node = this._ropeNodes[i];
            if (node == null) continue;

            if (!running) this.resetRope();
            else node.body.bodyType = RigidbodyType2D.Dynamic;
        }

        this._timeRunning = running;
    }

    private void resetRope() {
        this.end.transform.position = this._end_originalPosition;
        this.end.transform.localPosition = this._end_originalLocalPosition;
        this.end.transform.rotation = this._end_originalAngle;

        for (int i = 0; i < this._ropeNodes.Count; i++) {
            logic_rope_node node = this._ropeNodes[i];
            if (node == null) continue;
            if (node.isTEMP) {
                this._ropeNodes.Remove(node);
                Destroy(node.gameObject);
            } else {
                node.resetNode();
            }
        }
    }

    /* ************* 
     * Rope cutting
     ===============*/
    public void onRopeCut(logic_rope_node rope_node, Vector3 localCutPoint, Vector3 worldCutPoint) {
        if (rope_node == null || rope_node.joint == null) return;
        if (!this._timeRunning) return;

        logic_rope_node next_rope_node = rope_node.nextNode;
        if (next_rope_node == null) return;
        
        // Update the current rope with the cut position
        rope_node.transform.position = worldCutPoint;
        rope_node.joint.anchor -= new Vector2(0, localCutPoint.y);
        rope_node.body.bodyType = RigidbodyType2D.Dynamic;

        rope_node.updateNode();

        // Create a new rope
        logic_rope_node ropeStart = this.createStartNode(rope_node.gameObject, RigidbodyType2D.Dynamic);
        ropeStart.transform.localPosition = new Vector3(localCutPoint.x, localCutPoint.y, this.transform.position.z); // Fix Z
        ropeStart.transform.position = new Vector3(worldCutPoint.x, worldCutPoint.y, this.transform.position.z); // Fix Z
        ropeStart.isTEMP = true;

        // Create a new rope node
        logic_rope_node newNode = this.createRopeNode(ropeStart, worldCutPoint - localCutPoint);
        newNode.body.bodyType = RigidbodyType2D.Dynamic;
        newNode.isTEMP = true;

        next_rope_node.joint.connectedBody = newNode.body;
        next_rope_node.updateNode();
        
        ropeStart.nextNode = newNode;
        newNode.nextNode = next_rope_node;
        rope_node.nextNode = null;

        this._ropeNodes.Add(ropeStart);
        this._ropeNodes.Add(newNode);

        // Remove mass
        this.setRopeMass(ropeStart, 5f);
    }

    /* ************* 
     * ROPE UTIL
     ===============*/
    public void setRopeMass(logic_rope_node node, float newMass) {
        if (node == null || node.nextNode == null) return;
        node.body.mass = newMass;

        // Recursive
        this.setRopeMass(node.nextNode, newMass);
    }

    /* ************* 
     * CREATION
     ===============*/

    public logic_rope_node createStartNode(GameObject originalNode, RigidbodyType2D bodyType) {
        // CREATE A TEMP NODE
        int index = this._ropeNodes.Count;

        GameObject temp = new GameObject();
        temp.name = "rope_node_START_" + index;
        temp.transform.parent = this.gameObject.transform;
        temp.tag = "particle_object";

        Vector3 pos = originalNode.transform.position;
        temp.transform.position = new Vector3(pos.x, pos.y, this.transform.position.z); // Fix Z
        temp.layer = 13;

        Rigidbody2D tempBody = temp.AddComponent<Rigidbody2D>();
        tempBody.bodyType = bodyType;
        tempBody.mass = 100f;
        tempBody.angularDrag = 1f;

        logic_rope_node node = temp.AddComponent<logic_rope_node>();
        node.setRopeController(this);
        node.body = tempBody;

        return node;
    }

    public void generateRope() {
        // GET TOTAL NODES
        int totalNodes = this.getTotalNodes();

        // GENERATE NODES
        this._ropeNodes.Clear();
        this._ropeNodes.Add(this.createStartNode(this.gameObject, RigidbodyType2D.Kinematic)); // Add start

        // Loop between positions and create a node
        for (int i = 1; i <= totalNodes; i++) {
            logic_rope_node prevNode = this._ropeNodes[i - 1];
            Vector3 nodePos = this.getNodePosition(i);

            this._ropeNodes.Add(this.createRopeNode(prevNode, nodePos));
        }

        // Fix last node
        logic_rope_node lastNode = this._ropeNodes[totalNodes];

        HingeJoint2D joint = this.createJoint(lastNode.body, end);
        joint.anchor = joint.anchor - new Vector2(0, ropeOffset.y);

        this._endNode.body = this._endBody;
        this._endNode.joint = joint;


        // Add end body
        this._ropeNodes.Add(this._endNode); // Add start
        this.saveAndAssignNodes();
    }
    

    private logic_rope_node createRopeNode(logic_rope_node prevNode, Vector3 nodePos) {
        int index = this._ropeNodes.Count;

        // Main
        GameObject node = new GameObject();
        node.name = "rope_node_" + index;
        node.transform.parent = this.gameObject.transform;
        node.transform.position = new Vector3(nodePos.x, nodePos.y, this.transform.position.z); // Fix Z
        node.layer = 13;
        node.tag = "particle_object";

        Rigidbody2D body = node.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.mass = 100f;
        body.angularDrag = 1f;
        
        HingeJoint2D joint = this.createJoint(prevNode.body, node);

        LineRenderer lineRender = node.AddComponent<LineRenderer>();
        lineRender.receiveShadows = false;
        lineRender.shadowCastingMode = ShadowCastingMode.Off;
        lineRender.lightProbeUsage = LightProbeUsage.Off;
        lineRender.reflectionProbeUsage = ReflectionProbeUsage.Off; 
        lineRender.useWorldSpace = false;
        lineRender.textureMode = LineTextureMode.Tile;
        lineRender.sortingLayerName = "Background";
        lineRender.sharedMaterial = ropeMaterial;

        lineRender.positionCount = 2;
        lineRender.SetPosition(0, joint.anchor);
        lineRender.SetPosition(1, joint.connectedAnchor);

        lineRender.widthMultiplier = this.ropeWidth;

        EdgeCollider2D col = node.AddComponent<EdgeCollider2D>();
        col.isTrigger = true;
        col.points = new Vector2[] { joint.anchor, joint.connectedAnchor };
        col.edgeRadius = 0.03f;

        logic_rope_node logic_node = node.AddComponent<logic_rope_node>();
        logic_node.setRopeController(this);

        logic_node.col = col;
        logic_node.line = lineRender;
        logic_node.body = body;
        logic_node.joint = joint;

        return logic_node;
    }
    
    private HingeJoint2D createJoint(Rigidbody2D body, GameObject node) {
        HingeJoint2D joint = node.AddComponent<HingeJoint2D>();
        joint.anchor = (body.transform.position - node.transform.position);
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = Vector3.zero;
        joint.connectedBody = body;
        joint.enableCollision = false;

        return joint;
    }

    public void saveAndAssignNodes() {
        for (int i = 0; i < this._ropeNodes.Count; i++) {
            if (i + 1 >= this._ropeNodes.Count) break;
            this._ropeNodes[i].nextNode = this._ropeNodes[i + 1];
        }

        for (int i = 0; i < this._ropeNodes.Count; i++) {
            this._ropeNodes[i].saveNode();
        }
    }

    /* ************* 
     * NODE UTIL
     ===============*/
    private Vector3 getNodePosition(int index) {
        Vector3 startPos = this.transform.position;
        Vector3 endPos = end.transform.position + ropeOffset;
        Vector3 direction = (endPos - startPos);
        if (index == this.getTotalNodes()) return endPos;

        return startPos + direction.normalized * (ropeNodeLength * index);
    }

    private int getTotalNodes() {
        Vector3 startPos = this.transform.position;
        Vector3 endPos = end.transform.position + ropeOffset;

        float distance = Vector3.Distance(startPos, endPos);
        return Mathf.RoundToInt(distance / ropeNodeLength);
    }

    /* ************* 
     * DEBUG
     ===============*/

    public void OnDrawGizmos() {
        if (this.end == null) return;

        int totalNodes = this.getTotalNodes();

        Gizmos.color = Color.blue;
        for(int i = 0; i <= totalNodes; i++)
            Gizmos.DrawCube(this.getNodePosition(i), new Vector3(0.1f,0.1f,0));

        Gizmos.color = Color.red;
        Vector3 A = this.gameObject.transform.position;
        Vector3 D = this.end.transform.position + ropeOffset;

        Gizmos.DrawLine(A, D);
    }
}
