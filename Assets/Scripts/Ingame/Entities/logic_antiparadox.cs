
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class logic_antiparadox : MonoBehaviour {

    public Material renderMaterial;
    public Material lineMaterial;

    [HideInInspector]
    public bool displayParadox;

    private PolygonCollider2D _collider;
    private MeshFilter _filter;
    private MeshRenderer _renderer;
    private LineRenderer _lineRenderer;

    private float _targetAlpha;
    private float _currentAlpha;
    private float _lerpTime;

    /* ************* 
     * SETUP
     ===============*/
    public void Awake() {
        this._collider = GetComponent<PolygonCollider2D>();
        this._filter = GetComponent<MeshFilter>();
        this._renderer = GetComponent<MeshRenderer>();
        this._lineRenderer = GetComponent<LineRenderer>();
        
        this.gameObject.layer = 9;
        this.name = "logic_antiparadox";
        
        this._renderer.receiveShadows = false;
        this._renderer.shadowCastingMode = ShadowCastingMode.Off;
        this._renderer.lightProbeUsage = LightProbeUsage.Off;
        this._renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        this._renderer.sharedMaterial = renderMaterial;

        this._lineRenderer.receiveShadows = false;
        this._lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
        this._lineRenderer.lightProbeUsage = LightProbeUsage.Off;
        this._lineRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        this._lineRenderer.useWorldSpace = false;
        this._lineRenderer.textureMode = LineTextureMode.Tile;
        this._lineRenderer.sortingLayerName = "Background";
        this._lineRenderer.sharedMaterial = lineMaterial;

        Mesh paradoxMesh = this.buildMesh();
        this._filter.sharedMesh = paradoxMesh;

        this.buildLineRenderer(paradoxMesh);
    }


    public void Start() {
        if (!Application.isPlaying) this.setParadoxVisibility(true);
        else this.setParadoxVisibility(false);

        this._lerpTime = 0.9f; // Skip
    }

    /* ************* 
     * Display Effect
     ===============*/
    public void setParadoxVisibility(bool display) {
        if (this.displayParadox == display) return;

        this.displayParadox = display;
        this._currentAlpha = Mathf.Abs(this.lineMaterial.GetFloat("_particle_alpha")); // Either one, does not matter
        this._lerpTime = 0;
        
        if (!display) this._targetAlpha = 1f;
        else this._targetAlpha = 0f;
    }


    public void Update() {
        if (this.lineMaterial == null || this.renderMaterial == null) return;
        if (this._lerpTime >= 1) return;

        this._currentAlpha = Mathf.Lerp(this._currentAlpha, this._targetAlpha, this._lerpTime);
        if (this._currentAlpha <= 0.2f) this._currentAlpha = 0.2f;

        this._lerpTime += 0.01f;

        this.lineMaterial.SetFloat("_particle_alpha", -this._currentAlpha); 
        this.renderMaterial.SetFloat("_particle_alpha", -this._currentAlpha);
    }

    /* ************* 
     * Build Meshes
     ===============*/
    private void buildLineRenderer(Mesh mesh) {
        if (mesh == null) return;

        this._lineRenderer.positionCount = mesh.vertexCount;
        this._lineRenderer.SetPositions(mesh.vertices);
        this._lineRenderer.loop = true;
        this._lineRenderer.widthMultiplier = 0.2f;
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

        return genMesh;
    }

    /* ************* 
     * EDITOR
     ===============*/
    void OnDrawGizmos() {
        Gizmos.color = new Color(0, 255, 0, 255);
        //Gizmos.DrawIcon(transform.position, "logic_rockjump", true);
    }
}