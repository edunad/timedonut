
using UnityEngine;

public class util_drag : ScriptableObject {
    public delegate void dragEvent();
    public event dragEvent OnDrag;

    public bool isDragging;
    public bool isDisabled;

    private Camera _camera;
    private Rigidbody2D _body;

    private Vector3 _dragOffset;

    public void Awake() {
        this._camera = GameObject.Find("Camera").GetComponent<Camera>();
    }

    public void setup(Rigidbody2D body) {
        this._body = body;
    }

    public bool onMouseDown(Transform obj) {
        if (!this.isMouseOnObject() || this.isDisabled) return false;

        Vector3 pos = obj.transform.position;
        this._dragOffset = pos - this._camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pos.z));
        this.isDragging = true;

        return true; // If sucessfull
    }

    public bool onMouseDrag(Transform obj) {
        if (!this.isDragging) return false;

        Vector3 curPosition = this._camera.ScreenToWorldPoint(Input.mousePosition) + this._dragOffset;
        curPosition.z = obj.position.z; // Fix z

        obj.position = curPosition;
        if(OnDrag != null) this.OnDrag();

        return true;
    }

    public bool onMouseUp() {
        if (!this.isDragging) return false;

        this.isDragging = false;
        return true;
    }

    public bool isMouseOnObject() {
        if (this.isDisabled) return false;

        Vector3 mousePos = Input.mousePosition - new Vector3(1f, 1f, 0);
        RaycastHit2D screenRay = Physics2D.Raycast(this._camera.ScreenToWorldPoint(mousePos), Vector2.zero);
        if (screenRay.rigidbody == null || screenRay.rigidbody != this._body) return false;

        return true;
    }
}
