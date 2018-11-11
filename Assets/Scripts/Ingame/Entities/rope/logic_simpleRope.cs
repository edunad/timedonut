using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody2D))]
public class logic_simpleRope : MonoBehaviour {

    //Objects that will interact with the rope
    public GameObject end;

    public Material ropeMaterial;

    public float ropeWidth;
    public float ropeResolution;

    public Vector3 offset;

    private LineRenderer _lineRenderer;
    private HingeJoint2D _spring;

    private List<Vector3> _ropeSection;
    private Vector3 _oldStart;
    private Vector3 _oldEnd;

    public void Awake() {
        this._ropeSection = new List<Vector3>();
        this.setupRope();
    }

    public void Update() {
        this.renderUpdate();
    }

    private void setupRope() {
        this.gameObject.layer = 13;
        this.end.layer = 13;

        this._lineRenderer = this.gameObject.AddComponent<LineRenderer>();
        this._lineRenderer.receiveShadows = false;
        this._lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
        this._lineRenderer.lightProbeUsage = LightProbeUsage.Off;
        this._lineRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        this._lineRenderer.useWorldSpace = true;
        this._lineRenderer.textureMode = LineTextureMode.Tile;
        this._lineRenderer.sortingLayerName = "Background";
        this._lineRenderer.sharedMaterial = ropeMaterial;
        this._lineRenderer.widthMultiplier = ropeWidth;

        Rigidbody2D sBody = this.gameObject.GetComponent<Rigidbody2D>();

        Vector3 endPos = this.end.transform.position;
        Vector3 startPos = this.transform.position;

        this._spring = this.end.AddComponent<HingeJoint2D>();
        this._spring.connectedBody = sBody;
        this._spring.anchor = endPos - startPos;
        //this._spring.autoConfigureDistance = false;
        this._spring.autoConfigureConnectedAnchor = false;
        this._spring.connectedAnchor = Vector3.zero;
        //this._spring.distance = Vector3.Distance(this.gameObject.transform.position, this.end.transform.position + offset);
    }

    private void renderUpdate() {
        // Bezier curve formula ()
        Vector3 A = this.gameObject.transform.position;
        Vector3 D = this.end.transform.position + offset;

        if (this._oldStart == A && this._oldEnd == D) return;
        this._oldStart = A;
        this._oldEnd = D;

        Vector3 B = A + gameObject.transform.up * (-(A - D).magnitude * 0.1f);
        Vector3 C = D + end.transform.up * ((A - D).magnitude * 0.5f);

        this._ropeSection.Clear();

        // Generate the nodes
        float time = 0f;
        while (time <= 1f) {
            this._ropeSection.Add(this.DeCasteljausAlgorithm(A, B, C, D, time));
            time += Mathf.Clamp(ropeResolution, 0.0001f, 1f);
        }

        this._ropeSection.Add(this.DeCasteljausAlgorithm(A, B, C, D, 1f));

        this._lineRenderer.positionCount = this._ropeSection.Count;
        this._lineRenderer.SetPositions(this._ropeSection.ToArray());
    }

    //The De Casteljau's Algorithm
    private Vector3 DeCasteljausAlgorithm(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t) {
        float oneMinusT = 1f - t;

        //Layer 1
        Vector3 Q = oneMinusT * A + t * B;
        Vector3 R = oneMinusT * B + t * C;
        Vector3 S = oneMinusT * C + t * D;

        //Layer 2
        Vector3 P = oneMinusT * Q + t * R;
        Vector3 T = oneMinusT * R + t * S;

        //Final interpolated position
        return oneMinusT * P + t * T;
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.blue;

        Vector3 A = this.gameObject.transform.position;
        Vector3 D = this.end.transform.position + offset;
        Gizmos.DrawLine(A, D);
    }
}
