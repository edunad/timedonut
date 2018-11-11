using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class logic_rope_node {
    public GameObject node;
    public Rigidbody2D body;
    public HingeJoint2D joint;
    public LineRenderer line;

    public logic_rope_node(GameObject node, Rigidbody2D body, HingeJoint2D joint = null, LineRenderer line = null) {
        this.node = node;
        this.body = body;
        this.joint = joint;
        this.line = line;
    }
}

[RequireComponent(typeof(Rigidbody2D))]
public class logic_rope : MonoBehaviour {
    public GameObject end;

    public Vector3 ropeOffset;
    public float ropeWidth = 0.05f;
    public float ropeNodeLength = 0.6f;

    public Material ropeMaterial;

    private Rigidbody2D _startBody;
    private Rigidbody2D _endBody;

    private List<logic_rope_node> _ropeNodes;

    public void Awake() {
        this._ropeNodes = new List<logic_rope_node>();

        this._startBody = this.GetComponent<Rigidbody2D>();
        this._endBody = this.end.GetComponent<Rigidbody2D>();

        this.generateRope();
    }
    
    /* ************* 
     * Time related
     ===============*/
    public void enableMovement(bool enable) {
        for (int i = 0; i < this._ropeNodes.Count; i++) {
            logic_rope_node node = this._ropeNodes[i];
            if (node == null) continue;
            if (!enable) this.generateRope();

            if (node.node == this.gameObject || node.node == end) continue;
            node.body.bodyType = enable ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
        }
    }

    public void onRopeCut(GameObject cutter, GameObject node, Vector3 cutPoint) {
        if (node.name.IndexOf("rope_node_") == -1) return;
        int index = Convert.ToInt32(node.name.Replace("rope_node_", ""));

        logic_rope_node rope_node = this._ropeNodes[index];
        if (rope_node == null || rope_node.joint == null) return;

        // Fix prev node
        if (index - 1 > 0) {
            logic_rope_node prev_node = this._ropeNodes[index - 1];
            if (prev_node == null || prev_node.joint == null || prev_node.line == null) return;
            // MERGE THIS //
            logic_rope_node topSplit = this.createTempNode(index, prev_node.node);

            HingeJoint2D joint = this.createJoint(prev_node.body, topSplit.node);
            joint.anchor = prev_node.node.transform.localPosition - cutPoint;

            LineRenderer lineRender = topSplit.node.AddComponent<LineRenderer>();
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



            this._ropeNodes.Insert(index, topSplit);
        }

        logic_rope_node splitNode = this.createTempNode(index, rope_node.node);
        rope_node.joint.connectedBody = splitNode.body;

        this._ropeNodes.Insert(index, splitNode);
    }

    /* ************* 
     * CREATION
     ===============*/

    public logic_rope_node createTempNode(int index, GameObject originalNode) {
        // CREATE A TEMP NODE
        GameObject temp = new GameObject();
        temp.name = "rope_node_TEMP_" + index;
        temp.transform.parent = this.gameObject.transform;
        temp.transform.position = originalNode.transform.position;
        temp.layer = 13;

        Rigidbody2D tempBody = temp.AddComponent<Rigidbody2D>();
        tempBody.bodyType = RigidbodyType2D.Dynamic;
        tempBody.mass = 100f;
        tempBody.angularDrag = 10f;

        return new logic_rope_node(temp, tempBody, null);
    }

    public void generateRope() {
        // GET TOTAL NODES
        int totalNodes = this.getTotalNodes();

        // CLEANUP
        for (int i = 1; i < this._ropeNodes.Count - 1; i++) {
            GameObject.Destroy(this._ropeNodes[i].node);
        }

        // GENERATE NODES
        this._ropeNodes.Clear();
        this._ropeNodes.Add(new logic_rope_node(this.gameObject, this._startBody, null)); // Add start

        // Loop between positions and create a node
        for (int i = 1; i <= totalNodes; i++) {
            logic_rope_node prevNode = this._ropeNodes[i - 1];
            this._ropeNodes.Add(this.createRopeNode(i, prevNode.body));
        }

        // Fix last node
        logic_rope_node lastNode = this._ropeNodes[totalNodes];
        HingeJoint2D joint = this.createJoint(lastNode.body, end);
        joint.anchor = joint.anchor - new Vector2(0, ropeOffset.y);

        // Add end body
        this._ropeNodes.Add(new logic_rope_node(this.end, this._endBody, joint)); // Add start
    }

    private logic_rope_node createRopeNode(int index, Rigidbody2D prevBody) {
        Vector3 nodePos = this.getNodePosition(index);

        // Main
        GameObject node = new GameObject();
        node.name = "rope_node_" + index;
        node.transform.parent = this.gameObject.transform;
        node.transform.position = nodePos;
        node.layer = 13;

        Rigidbody2D body = node.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.mass = 100f;
        body.angularDrag = 10f;
        
        HingeJoint2D joint = this.createJoint(prevBody, node);

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

        return new logic_rope_node(node, body, joint, lineRender);
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
