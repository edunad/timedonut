using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(PolygonCollider2D))]
public class logic_antiparadox : MonoBehaviour {

    private PolygonCollider2D _collider;
    private MeshFilter _filter;
    private MeshRenderer _renderer;

    public void Awake() {
        this._collider = GetComponent<PolygonCollider2D>();
        this._filter = GetComponent<MeshFilter>();
        this._renderer = GetComponent<MeshRenderer>();

        this.gameObject.layer = 9;
        this.name = "logic_antiparadox";

        Mesh msh = this.buildMesh();
        if (msh == null) return;
        this._filter.mesh = msh;
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
        Vector3[] vertices = new Vector3[paths.Length];
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = new Vector3(paths[i].x, paths[i].y, 0);
        }

        genMesh.vertices = vertices;
        genMesh.triangles = indices;
        genMesh.RecalculateNormals();
        genMesh.RecalculateBounds();

        return genMesh;
    }

    void OnDrawGizmos() {
        Gizmos.color = new Color(0, 255, 0, 255);
        //Gizmos.DrawIcon(transform.position, "logic_rockjump", true);
    }
}
