
using UnityEngine;

public class util_resetable : MonoBehaviour {
    private Vector3 _originalPosition;
    private Vector3 _originalLocalPosition;
    private Quaternion _originalAngle;
    private Quaternion _originalLocalAngle;

    private Transform _originalParent;

    public void saveObject() {
        this._originalPosition = this.transform.position;
        this._originalLocalPosition = this.transform.localPosition;

        this._originalAngle = this.transform.rotation;
        this._originalLocalAngle = this.transform.localRotation;

        this._originalParent = this.transform.parent;
    }

    public void resetObject() {
        this.transform.position = this._originalPosition;
        this.transform.localPosition = this._originalLocalPosition;

        this.transform.rotation = this._originalAngle;
        this.transform.localRotation = this._originalLocalAngle;

        this.transform.parent = this._originalParent;
    }
}
