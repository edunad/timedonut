using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(LineRenderer))]
public class logic_antiparadox : MonoBehaviour {

    private PolygonCollider2D _collider;
    private MeshFilter _filter;
    private MeshRenderer _renderer;
    private Material _renderMaterial;
    private Camera _camera;
    private LineRenderer _lineRenderer;

    public void Awake() {
        this._collider = GetComponent<PolygonCollider2D>();
        this._filter = GetComponent<MeshFilter>();
        this._renderer = GetComponent<MeshRenderer>();
        this._renderMaterial = this._renderer.material;
        this._camera = GameObject.Find("Camera").GetComponent<Camera>();
        this._lineRenderer = GetComponent<LineRenderer>();


        this.gameObject.layer = 9;
        this.name = "logic_antiparadox";

        Mesh msh = this.buildMesh();
        if (msh == null) return;
        this._filter.mesh = msh;
 
    }


    public void Update() {
        Vector3 curPosition = this._camera.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(curPosition);
    }

    private Mesh buildMesh() {
        int maxPoints = this._collider.pathCount;
        if (maxPoints <= 0) return null;

        Mesh genMesh = new Mesh();
        Vector2[] paths = this._collider.GetPath(0);
        Triangulator tr = new Triangulator(paths);

        // Generate
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i < paths.Length; i++) {
            vertices.Add(new Vector3(paths[i].x, paths[i].y, 0));
        }
       
        genMesh.SetVertices(vertices);
        genMesh.RecalculateBounds();

        Bounds bounds = genMesh.bounds;
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < vertices.Count; i++) {
            uvs.Add(new Vector2(vertices[i].x / bounds.size.x, vertices[i].y / bounds.size.y));
        }
        
        genMesh.SetUVs(0, uvs);
        genMesh.SetTriangles(indices, 0);
        genMesh.RecalculateNormals();

        this._lineRenderer.positionCount = vertices.Count;
        this._lineRenderer.SetPositions(vertices.ToArray());
        this._lineRenderer.loop = true;
        this._lineRenderer.widthMultiplier = 0.1f;

        return genMesh;
    }

    void OnDrawGizmos() {
        Gizmos.color = new Color(0, 255, 0, 255);
        //Gizmos.DrawIcon(transform.position, "logic_rockjump", true);
    }
}
