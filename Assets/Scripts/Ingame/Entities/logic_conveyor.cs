using Assets.Scripts.models;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(EdgeCollider2D))]
public class logic_conveyor : MonoBehaviour {

    [Header("Conveyor settings")]
    public float speed;

    [Header("Conveyor rendering")]
    public float conveyorSize = 0.3f;
    public Material conveyorMaterial;
    public Texture beltTexture;
    public Vector3 beltOffset = Vector3.zero;

    [SerializeField]
    public List<Vector3> conveyorOffset = new List<Vector3>();

    private bool _isActive;
    private float _originalSpeed;
    private EdgeCollider2D _collision;

    private Material _beltMaterial;

    private List<Collider2D> _colliders;
    private List<Rigidbody2D> _collidersBodies;

    public void Awake() {
        // Conveyor maker 2000
        this.name = "logic_conveyor";
        this.tag = "conveyor_object";
        this.gameObject.layer = 8; // Static

        this._colliders = new List<Collider2D>();
        this._collidersBodies = new List<Rigidbody2D>();

        this._collision = this.GetComponent<EdgeCollider2D>();

        // Generate belt
        this._beltMaterial = new Material(Shader.Find("Conveyor_belt_shader"));
        this._beltMaterial.mainTexture = this.beltTexture;

        this.createBelt(this._beltMaterial, new Vector3(0f, 0.07f, 0) + beltOffset, (this.conveyorSize - 0.11f), 9);
        this.createBelt(this.conveyorMaterial, Vector3.zero, this.conveyorSize, 10);

        // COLLISION
        this._originalSpeed = speed;
        this.generateCollision();
    }

    /* ************* 
     * GENERATION
     ===============*/
    private void generateCollision() {
        List<Vector2> fixedPoints = new List<Vector2>() {
            Vector2.zero
        };

        Vector3 val = Vector3.zero;
        foreach (Vector3 point in this.conveyorOffset) {
            val += point;
            fixedPoints.Add(new Vector2(val.x, val.y));
        }

        this._collision.points = fixedPoints.ToArray();
        this._collision.edgeRadius = (this.conveyorSize - 0.11f);
    }

    private void createBelt(Material mat, Vector3 offset, float size, int order) {
        GameObject objs = new GameObject();
        objs.name = "conveyor";
        objs.transform.parent = this.transform;
        objs.transform.position = this.transform.position;
        objs.transform.localPosition = offset;

        LineRenderer _belt = objs.AddComponent<LineRenderer>();
        _belt.widthMultiplier = size;
        _belt.receiveShadows = false;
        _belt.shadowCastingMode = ShadowCastingMode.Off;
        _belt.lightProbeUsage = LightProbeUsage.Off;
        _belt.reflectionProbeUsage = ReflectionProbeUsage.Off;
        _belt.useWorldSpace = false;
        _belt.textureMode = LineTextureMode.Tile;
        _belt.sortingLayerName = "PlayArea";
        _belt.sharedMaterial = mat;
        _belt.numCapVertices = 0;
        _belt.numCornerVertices = 2;
        _belt.sortingOrder = order;

        // Line render!
        List<Vector3> points = this.getPoints();

        _belt.positionCount = points.Count;
        _belt.SetPositions(points.ToArray());
    }

    private void setBelt(bool enabled) {
        this._beltMaterial.SetFloat("_belt_speed", enabled ? speed : 0);
    }

    private List<Vector3> getPoints() {
        List<Vector3> points = new List<Vector3>() {
            Vector3.zero
        };

        Vector3 pos = Vector3.zero;
        for (int i = 0; i < this.conveyorOffset.Count; i++) {
            pos += this.conveyorOffset[i];
            points.Add(pos);
        }

        return points;
    }

    /* ************* 
     * COLLISION
     ===============*/
    public void OnCollisionEnter2D(Collision2D collision) {
        Collider2D col = collision.collider;
        if (col == null || this._colliders.Contains(col)) return;

        this._colliders.Add(col);
        this._collidersBodies.Add(col.GetComponent<Rigidbody2D>());
    }

    public void OnCollisionExit2D(Collision2D collision) {
        Collider2D col = collision.collider;
        if (col == null || !this._colliders.Contains(col)) return;

        this._collidersBodies.Remove(col.GetComponent<Rigidbody2D>());
        this._colliders.Remove(col);
    }

    /* ************* 
     * Push objects
     ===============*/
    public void Update() {
        if (!this._isActive) return;
        if (this._colliders == null || this._colliders.Count <= 0) return;
        foreach (Rigidbody2D body in _collidersBodies) {
            if (body == null || body.bodyType != RigidbodyType2D.Dynamic) continue;
            body.MovePosition(body.position + Vector2.left * speed * Time.deltaTime);
        }
    }

    /* ************* 
     * EVENTS + TIME
     ===============*/
    public void OnEnable() {
        CoreController.OnTimeChange += this.setTimeStatus;
        CoreController.OnGameWin += this.setVictory;
    }

    public void OnDisable() {
        CoreController.OnTimeChange -= this.setTimeStatus;
        CoreController.OnGameWin -= this.setVictory;
    }

    private void setVictory() {
        this.setTimeStatus(false);
    }

    private void setTimeStatus(bool running) {
        this._collidersBodies.Clear();
        this._colliders.Clear();

        if (!running) {
            this._isActive = false;
            this.speed = this._originalSpeed; // Reset speed
            this.setBelt(false);
        }
    }


    /* ************* 
     * NETWORKING
     ===============*/
    public void onDataRecieved(network_data msg) {
        if (msg == null) return;

        if (msg.header == "active") {
            this._isActive = (msg.data == 1);
        }else if(msg.header == "reverse") {
            this.speed = -this.speed;
        }

        this.setBelt(this._isActive);
    }

    /* ************* 
     * DEBUG
     ===============*/
    public void OnDrawGizmos() {
        if (this.conveyorOffset == null) return;

        Gizmos.color = Color.cyan;
        Vector3 pos = this.transform.position;

        for (int i = 0; i < this.conveyorOffset.Count; i++) {
            Vector3 newPos = pos + this.conveyorOffset[i];
            Gizmos.DrawLine(pos, newPos);
            Gizmos.DrawSphere(pos, 0.05f);

            pos += this.conveyorOffset[i];
        }
        
        Gizmos.DrawSphere(pos, 0.05f);
        Gizmos.DrawIcon(this.transform.position, "gizmo_conveyor");
    }
}
