using Assets.Scripts.models;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class logic_door : MonoBehaviour {

    [Header("Door settings")]
    public Vector2 offset;
    public float doorSpeed;
    public float openTime = 0f;

    private Vector3 _originalPos;
    private Vector3 _endPos;

    private float _startTime;
    private bool _enabled;
    private util_timer _timer;

    public void Awake () {
        this.name = "logic_door";
        this.tag = "door_object";

        this._originalPos = this.transform.position;
        this._endPos = this._originalPos + new Vector3(offset.x, offset.y, 0);
    }

    /* ************* 
     * EVENTS + TIME
     ===============*/

    public void onTimeChange(bool isActive) {
        this.transform.position = this._originalPos;

        this._startTime = 0f;
        this._enabled = false;

        if (this._timer != null) this._timer.Stop();
    }

    public void OnEnable() {
        CoreController.OnTimeChange += this.onTimeChange;
    }

    public void OnDisable() {
        CoreController.OnTimeChange -= this.onTimeChange;
    }

    /* ************* 
     * LOGIC
     ===============*/
    public void onDataRecieved(network_data msg) {
        if (msg == null || msg.header != "active") return;
        bool enabled = msg.data == 1;

        if (enabled && openTime > 0) {
            if (this._timer != null) this._timer.Stop();
            this._timer = util_timer.Simple(openTime, () => {
                this._enabled = false;
            });
        }

        this._enabled = enabled;
    }

    public void Update() {
        // Pretty sure lerp could do this, but it doesn't want to talk with me :(
        if (this._enabled && this._startTime < 1f) this._startTime += doorSpeed;
        else if (!this._enabled && this._startTime > 0f) this._startTime -= doorSpeed;
        else return;

        this._startTime = Mathf.Clamp(this._startTime, 0f, 1f);
        this.transform.position = Vector3.Lerp(this._originalPos, this._endPos, this._startTime);
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(this.transform.position, new Vector3(0.1f, 0.1f, 0.1f));

        Gizmos.color = Color.red;
        Vector3 endPoint = this.transform.position + new Vector3(offset.x, offset.y, 0);
        Gizmos.DrawLine(this.transform.position, endPoint);
        Gizmos.DrawCube(endPoint, new Vector3(0.1f, 0.1f, 0.1f));
    }
}
