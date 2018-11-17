
using UnityEngine;

public class ui_floating : MonoBehaviour {

    [Header("Timing")]
    public float speed = 2f;
    public float upOffset = 0.005f;

    private float _originalYPos;
	public void Awake () {
        this._originalYPos = this.transform.localPosition.y;
    }
    
    public void Update () {
        Vector3 pos = this.transform.localPosition;
        float y = Mathf.Sin(Time.time * this.speed) * this.upOffset;
        this.transform.localPosition = new Vector3(pos.x, this._originalYPos + y, pos.z);
    }
}
