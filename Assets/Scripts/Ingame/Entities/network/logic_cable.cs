
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(LineRenderer))]
public class logic_cable : MonoBehaviour {
    public Transform endPoint;
    public float cableSize = 0.05f;

    [SerializeField]
    public List<Vector3> cableOffset = new List<Vector3>();

    private Vector3 _startPoint;
    private LineRenderer _cable;
    private Material _material;

    public void Awake() {
        this._material = new Material(Shader.Find("Cable_shader"));

        this._cable = GetComponent<LineRenderer>();
        this._cable.widthMultiplier = cableSize;
        this._cable.receiveShadows = false;
        this._cable.shadowCastingMode = ShadowCastingMode.Off;
        this._cable.lightProbeUsage = LightProbeUsage.Off;
        this._cable.reflectionProbeUsage = ReflectionProbeUsage.Off;
        this._cable.useWorldSpace = true;
        this._cable.textureMode = LineTextureMode.DistributePerSegment;
        this._cable.sortingLayerName = "Foreground";
        this._cable.sharedMaterial = this._material;

        this._startPoint = this.transform.position;

        this.name = "logic_cable";
        this.tag = "particle_object";

        this.buildCable();
    }

    public void setCableColor(Color col) {
        if (this._material == null) return;
        this._material.SetColor("_cable_color", col);
    }

    private void buildCable() {

        List<Vector3> points = new List<Vector3>() {
            this._startPoint
        };

        Vector3 pos = this.transform.position;
        for (int i = 0; i < this.cableOffset.Count; i++) {
            pos += this.cableOffset[i];
            points.Add(pos);
        }

        points.Add(this.endPoint.position);

        // Line render!
        this._cable.positionCount = points.Count;
        this._cable.SetPositions(points.ToArray());

        this._cable.numCapVertices = 2;
        this._cable.numCornerVertices = 2;
    }


    public void OnDrawGizmos() {
        if (endPoint == null || this.cableOffset == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(this.transform.position, new Vector3(0.1f, 0.1f, 0));
        Gizmos.DrawCube(endPoint.position, new Vector3(0.1f, 0.1f, 0));

        Gizmos.color = Color.yellow;
        Vector3 pos = this.transform.position;

        for (int i = 0; i < this.cableOffset.Count; i++) {
            Vector3 newPos = pos + this.cableOffset[i];
            Gizmos.DrawLine(pos, newPos);
            Gizmos.DrawSphere(pos, 0.05f);

            pos += this.cableOffset[i];
        }

        Gizmos.DrawLine(pos, this.endPoint.position);
        Gizmos.DrawSphere(pos, 0.05f);
    }
}
