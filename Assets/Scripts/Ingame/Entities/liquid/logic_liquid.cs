using UnityEngine;
using System.Collections.Generic;

// ADAPTED FROM http://gamedevelopment.tutsplus.com/tutorials/creating-dynamic-2d-water-effects-in-unity--gamedev-14143

public enum LiquidType {
    LIQUID_WATER = 1,
    LIQUID_LAVA = 2,
    LIQUID_DONUT = 3
}

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(LineRenderer))]
public class logic_liquid : MonoBehaviour {

    // SETTINGS //
    [Header("Liquid Type")]
    public LiquidType liquidType = LiquidType.LIQUID_WATER;

    [Header("Main Settings")]
    public Vector2 liquidSize;
    public float liquidLineHeight = 0.05f;
    public float liquidYLimit = 0.15f;

    [Header("Simulation Settings")]
    public int liquidVertx = 4;
    public int liquidPasses = 2;

    [Header("Liquid Settings")]
    public float springconstant = 0.01f;
    public float damping = 0.06f;
    public float spread = 0.02f;
    public float forceMultiply = 0.1f;

    public Material liquidMaterial;
    public Material liquidTopMaterial;
    public Material liquidMiddleMaterial;

    [Header("Other Settings")]
    public GameObject splashParticle;

    // Constant //
    private const float z = -1f;

    // VARS //
    private float[] xpositions;
    private float[] ypositions;
    private float[] velocities;
    private float[] accelerations;

    private LineRenderer lineRender;

    private BoxCollider2D insideTrigger;

    private GameObject[] meshBottomobjects;
    private Mesh[] meshesBottom;

    private GameObject[] meshMiddleobjects;
    private Mesh[] meshesMiddle;

    private GameObject[] meshTopobjects;
    private Mesh[] meshesTop;

    private float baseheight;
    private float bottom;

    private util_timer splashTimer;

    private List<GameObject> _insideLiquidObjs;

    void Start() {
        _insideLiquidObjs = new List<GameObject>();

        // Setup BoxCollider //
        insideTrigger = GetComponent<BoxCollider2D>();
        insideTrigger.isTrigger = true;

        insideTrigger.size = new Vector2(liquidSize.x, Mathf.Abs(liquidSize.y));
        insideTrigger.offset = new Vector2(liquidSize.x / 2, -Mathf.Abs(liquidSize.y) / 2f);

        GenerateLiquid(transform.position.x, liquidSize.x, transform.position.y, transform.position.y + liquidSize.y);

        this.name = "logic_liquid";
    }

    public void Splash(int index, Vector3 pos, float force, GameObject obj = null) {
        if (index > velocities.Length - 1 || index < 0) return;

        // Prevent duplicates
        if (obj != null)
            if (_insideLiquidObjs.Contains(obj))
                return;

        // Apply force
        velocities[index] += force;

        #region Create Particle
        if (splashParticle != null) {
            GameObject particle = Instantiate<GameObject>(splashParticle);
            particle.transform.parent = transform;
            particle.transform.position = pos;

            ParticleSystem sys = particle.GetComponent<ParticleSystem>();
            sys.Play();

            util_timer.Simple(1f, () => { Destroy(particle); });
        }
        #endregion

        if (obj != null)
            _insideLiquidObjs.Add(obj);
    }

    public void GenerateLiquid(float Left, float Width, float Top, float Bottom) {
        int edgecount = liquidVertx;
        int nodecount = edgecount + 1;

        // Create Body //
        lineRender = GetComponent<LineRenderer>();
        Material mat = new Material(Shader.Find("Liquid_line_shader"));
        lineRender.sharedMaterial = mat;

        if (liquidType == LiquidType.LIQUID_WATER)
            mat.SetColor("_color", new Color(0.97f, 0.97f, 0.97f));
        else if (liquidType == LiquidType.LIQUID_LAVA)
            mat.SetColor("_color", new Color(0.69f, 0.27f, 0));
        else if (liquidType == LiquidType.LIQUID_DONUT)
            mat.SetColor("_color", new Color(0.69f, 0f, 0.28f));
        

        lineRender.sortingLayerName = "PlayArea";
        lineRender.sortingOrder = 40;
        lineRender.useWorldSpace = true;
        lineRender.receiveShadows = false;
        lineRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        lineRender.positionCount = nodecount;
        lineRender.startWidth = liquidLineHeight;
        lineRender.endWidth = liquidLineHeight;

        // Create the nodes //
        xpositions = new float[nodecount];
        ypositions = new float[nodecount];
        velocities = new float[nodecount];
        accelerations = new float[nodecount];

        meshBottomobjects = new GameObject[edgecount];
        meshesBottom = new Mesh[edgecount];

        meshMiddleobjects = new GameObject[edgecount];
        meshesMiddle = new Mesh[edgecount];

        meshTopobjects = new GameObject[edgecount];
        meshesTop = new Mesh[edgecount];

        baseheight = Top;
        bottom = Bottom;

        #region Generate Line Render
        for (int i = 0; i < nodecount; i++) {
            ypositions[i] = Top;
            xpositions[i] = Left + Width * i / edgecount;

            accelerations[i] = 0;
            velocities[i] = 0;
            lineRender.SetPosition(i, new Vector3(xpositions[i], ypositions[i], z - 1f));
        }
        #endregion

        #region Create Bottom Edges
        Vector3 pos = this.transform.position;
        for (int i = 0; i < edgecount; i++) {
            meshesBottom[i] = new Mesh();
            meshesTop[i] = new Mesh();
            meshesMiddle[i] = new Mesh();

            Vector3[] VerticesBottom = new Vector3[4];
            VerticesBottom[0] = new Vector3(xpositions[i], ypositions[i], z);
            VerticesBottom[1] = new Vector3(xpositions[i + 1], ypositions[i + 1], z);
            VerticesBottom[2] = new Vector3(xpositions[i], bottom, z);
            VerticesBottom[3] = new Vector3(xpositions[i + 1], bottom, z);

            Vector3[] VerticesTop = new Vector3[4];
            VerticesTop[0] = new Vector3(xpositions[i], ypositions[i] + 0.25f, z);
            VerticesTop[1] = new Vector3(xpositions[i + 1], ypositions[i] + 0.25f, z);
            VerticesTop[2] = new Vector3(xpositions[i], ypositions[i], z);
            VerticesTop[3] = new Vector3(xpositions[i + 1], ypositions[i], z);

            Vector3[] VerticesMiddle = new Vector3[4];
            VerticesMiddle[0] = new Vector3(xpositions[i], ypositions[i] + 0.5f, z + 0.5f);
            VerticesMiddle[1] = new Vector3(xpositions[i + 1], ypositions[i] + 0.5f, z + 0.5f);
            VerticesMiddle[2] = new Vector3(xpositions[i], ypositions[i] + 0.25f, z + 0.5f);
            VerticesMiddle[3] = new Vector3(xpositions[i + 1], ypositions[i] + 0.25f, z + 0.5f);

            Vector2[] UVs = new Vector2[4];
            UVs[0] = new Vector2(0, 1);
            UVs[1] = new Vector2(1, 1);
            UVs[2] = new Vector2(0, 0);
            UVs[3] = new Vector2(1, 0);

            int[] tris = new int[6] { 0, 1, 3, 3, 2, 0 };
            meshesBottom[i].vertices = VerticesBottom;
            meshesBottom[i].uv = UVs;
            meshesBottom[i].triangles = tris;

            meshesTop[i].vertices = VerticesTop;
            meshesTop[i].uv = UVs;
            meshesTop[i].triangles = tris;

            meshesMiddle[i].vertices = VerticesMiddle;
            meshesMiddle[i].uv = UVs;
            meshesMiddle[i].triangles = tris;

            #region Setup Bottom
            meshBottomobjects[i] = new GameObject("liquidMesh_BOTTOM");
            MeshRenderer renderBottom = meshBottomobjects[i].AddComponent<MeshRenderer>();
            renderBottom.material = liquidMaterial;
            renderBottom.sortingLayerName = "PlayArea";
            renderBottom.sortingOrder = 40;
            renderBottom.receiveShadows = false;
            renderBottom.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            meshBottomobjects[i].AddComponent<MeshFilter>().mesh = meshesBottom[i];

            logic_liquid_collider collider = meshBottomobjects[i].AddComponent<logic_liquid_collider>();
            collider._controller = this;
            collider.indx = i;

            meshBottomobjects[i].transform.parent = transform;
            #endregion

            #region Setup Top
            meshTopobjects[i] = new GameObject("liquidMesh_TOP");
            MeshRenderer renderTop = meshTopobjects[i].AddComponent<MeshRenderer>();
            renderTop.material = liquidTopMaterial;
            renderTop.sortingLayerName = "PlayArea";
            renderTop.sortingOrder = 40;
            renderTop.receiveShadows = false;
            renderTop.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            meshTopobjects[i].AddComponent<MeshFilter>().mesh = meshesTop[i];
            meshTopobjects[i].transform.parent = transform;
            #endregion

            #region Setup Middle

            meshMiddleobjects[i] = new GameObject("liquidMesh_MID");
            MeshRenderer renderMiddle = meshMiddleobjects[i].AddComponent<MeshRenderer>();
            renderMiddle.material = liquidMiddleMaterial;
            renderMiddle.sortingLayerName = "PlayArea";
            renderMiddle.sortingOrder = 10;
            renderMiddle.receiveShadows = false;
            renderMiddle.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            meshMiddleobjects[i].AddComponent<MeshFilter>().mesh = meshesMiddle[i];
            meshMiddleobjects[i].transform.parent = transform;

            #endregion
        }



        #endregion

        // Create the timer
        if (liquidType == LiquidType.LIQUID_LAVA) {
            splashTimer = util_timer.Create(0, 2f, () => {
                int indx = Random.Range(0, velocities.Length - 1);
                Vector3 poss = new Vector3(xpositions[indx], transform.position.y, transform.position.z);
                Splash(indx, poss, Random.Range(0.01f, forceMultiply));
            });
        }else if (liquidType == LiquidType.LIQUID_DONUT) {
            splashTimer = util_timer.Create(0, 5f, () => {
                int indx = Random.Range(0, velocities.Length - 1);
                Vector3 poss = new Vector3(xpositions[indx], transform.position.y, transform.position.z);
                Splash(indx, poss, Random.Range(0.01f, forceMultiply));
            });
        }
    }

    void OnDestroy() {
        if (splashTimer != null)
            splashTimer.Stop();
    }

    //Same as the code from in the meshes before, set the new mesh positions
    void UpdateMeshes() {
        for (int i = 0; i < meshesBottom.Length; i++) {
            Vector3[] VerticesBottom = new Vector3[4];
            Vector3[] VerticesTop = new Vector3[4];
            Vector3[] VerticesMiddle = new Vector3[4];

            float y = Mathf.Clamp(ypositions[i], -1000, liquidYLimit + transform.position.y);
            float ynext = Mathf.Clamp(ypositions[i + 1], -1000, liquidYLimit + transform.position.y);

            VerticesBottom[0] = new Vector3(xpositions[i], y, z);
            VerticesBottom[1] = new Vector3(xpositions[i + 1], ynext, z);
            VerticesBottom[2] = new Vector3(xpositions[i], bottom, z);
            VerticesBottom[3] = new Vector3(xpositions[i + 1], bottom, z);


            VerticesTop[0] = new Vector3(xpositions[i], y + 0.25f, z);
            VerticesTop[1] = new Vector3(xpositions[i + 1], ynext + 0.25f, z);
            VerticesTop[2] = new Vector3(xpositions[i], y, z);
            VerticesTop[3] = new Vector3(xpositions[i + 1], ynext, z);

            VerticesMiddle[0] = new Vector3(xpositions[i], y + 0.5f, z + 0.5f);
            VerticesMiddle[1] = new Vector3(xpositions[i + 1], ynext + 0.5f, z + 0.5f);
            VerticesMiddle[2] = new Vector3(xpositions[i], y + 0.25f, z + 0.5f);
            VerticesMiddle[3] = new Vector3(xpositions[i + 1], ynext + 0.25f, z + 0.5f);

            meshesTop[i].vertices = VerticesTop;
            meshesMiddle[i].vertices = VerticesMiddle;
            meshesBottom[i].vertices = VerticesBottom;
        }
    }

    public float SumArray(float[] toBeSummed) {
        float sum = 0f;
        foreach (float item in toBeSummed)
            sum += Mathf.Abs(item);
        return (float)System.Math.Round(sum, 4);
    }

    //Called regularly by Unity
    void FixedUpdate() {
        float sum = SumArray(velocities);
        if (sum <= 0) return;

        //Here we use the Euler method to handle all the physics of our springs:
        for (int i = 0; i < xpositions.Length; i++) {
            float force = springconstant * (ypositions[i] - baseheight) + velocities[i] * damping;
            accelerations[i] = -force;
            ypositions[i] += velocities[i];
            velocities[i] += accelerations[i];

            lineRender.SetPosition(i, new Vector3(xpositions[i], Mathf.Clamp(ypositions[i], -1000, liquidYLimit + transform.position.y), z - 1f));
        }

        //Now we store the difference in heights:
        float[] leftDeltas = new float[xpositions.Length];
        float[] rightDeltas = new float[xpositions.Length];

        for (int j = 0; j < liquidPasses; j++) {
            for (int i = 0; i < xpositions.Length; i++) {
                //We check the heights of the nearby nodes, adjust velocities accordingly, record the height differences
                if (i > 0) {
                    leftDeltas[i] = spread * (ypositions[i] - ypositions[i - 1]);
                    velocities[i - 1] += leftDeltas[i];
                }
                if (i < xpositions.Length - 1) {
                    rightDeltas[i] = spread * (ypositions[i] - ypositions[i + 1]);
                    velocities[i + 1] += rightDeltas[i];
                }
            }

            for (int i = 0; i < xpositions.Length; i++) {
                if (i > 0)
                    ypositions[i - 1] += leftDeltas[i];
                if (i < xpositions.Length - 1)
                    ypositions[i + 1] += rightDeltas[i];
            }
        }

        //Finally we update the meshes to reflect this
        UpdateMeshes();
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject == null) return;
        if (_insideLiquidObjs.Contains(other.gameObject)) {
            _insideLiquidObjs.Remove(other.gameObject);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = new Color(0, 150, 255);

        // Draw PIT
        Vector3 col = transform.position;

        float relativeX = col.x + liquidSize.x;
        float relativeY = col.y + liquidSize.y;
        float relativeLimitY = col.y + liquidYLimit;

        // Draw Lines
        Gizmos.DrawLine(new Vector3(col.x, col.y, col.z), new Vector3(relativeX, col.y, col.z));
        Gizmos.DrawLine(new Vector3(col.x + liquidSize.x / 2, col.y, col.z), new Vector3(col.x + liquidSize.x / 2, relativeY, col.z));
        Gizmos.DrawLine(new Vector3(col.x + liquidSize.x / 2, col.y, col.z), new Vector3(col.x + liquidSize.x / 2, relativeLimitY, col.z));

        // Draw Circles
        Gizmos.DrawSphere(col, 0.1f);
        Gizmos.DrawSphere(new Vector3(relativeX, col.y, col.z), 0.1f);
        Gizmos.DrawSphere(new Vector3(col.x + liquidSize.x / 2, relativeY, col.z), 0.1f);
        Gizmos.DrawSphere(new Vector3(col.x + liquidSize.x / 2, relativeLimitY, col.z), 0.1f);
    }

}
