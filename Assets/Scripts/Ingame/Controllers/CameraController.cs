using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {

    [Header("Camera zoom settings")]
    public bool zoomEnabled = true;
    public float sensitivity = 4f;
    public float minZoom = 3f;
    public float maxZoom = 4.76f;

    [Header("Camera playground bounds")]
    public bool playAreaEnabled = true;
    public Rect playArea;
    public Vector3 playAreaOffset;

    [Header("Camera controls")]
    public bool canControlCamera = true;
    public float controlSpeed = 2f;

    private Camera _camera;
    private Vector3 _camMovePos;

    public void Awake() {
        this._camera = GetComponent<Camera>();
        this._camera.orthographicSize = maxZoom; // Zoomout
    }

    /* ************* 
     * Object validation
     ===============*/
    public bool isObjectInsidePlayArea(GameObject obj) {
        Vector3 pos = obj.transform.position;

        float minPlayX = this.playArea.x + this.playAreaOffset.x;
        float minPlayY = this.playArea.y + this.playAreaOffset.y;

        float maxPlayX = this.playArea.width + this.playAreaOffset.x;
        float maxPlayY = this.playArea.height + this.playAreaOffset.y;
         
        if (pos.y < minPlayY || pos.y > maxPlayY) return false;
        if (pos.x < minPlayX || pos.x > maxPlayX) return false;
        return true;
    }

    /* ************* 
     * CAMERA CONTROLS
     ===============*/
    public void Update() {
        if (this._camera == null) return;

        #region Zoom
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && this.zoomEnabled && this.canControlCamera) {
            float currZoom = this._camera.orthographicSize + -Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            currZoom = Mathf.Clamp(currZoom, this.minZoom, this.maxZoom);
            this._camera.orthographicSize = currZoom;
        }
        #endregion

        #region Movement
        if (this.canControlCamera) {
            Vector3 camPos = this._camera.transform.position;

            if (Input.GetKey(KeyCode.D)) {
                camPos.x += this.controlSpeed;
            } else if (Input.GetKey(KeyCode.A)) {
                camPos.x -= this.controlSpeed;
            }

            if (Input.GetKey(KeyCode.W)) {
                camPos.y += this.controlSpeed;
            } else if (Input.GetKey(KeyCode.S)) {
                camPos.y -= this.controlSpeed;
            }

            this._camera.transform.position = camPos;
        }
        #endregion
    }

    public void LateUpdate() {
        if (this._camera == null && this.playAreaEnabled) return;

        // Prevent camera from going outside the bounds
        float minPlayX = this.playArea.x + this.playAreaOffset.x;
        float minPlayY = this.playArea.y + this.playAreaOffset.y;

        float maxPlayX = this.playArea.width + this.playAreaOffset.x;
        float maxPlayY = this.playArea.height + this.playAreaOffset.y;

        float camVertExtent = this._camera.orthographicSize;
        float camHorzExtent = this._camera.aspect * camVertExtent;

        float leftBound = minPlayX + camHorzExtent;
        float rightBound = maxPlayX - camHorzExtent;
        float bottomBound = minPlayY + camVertExtent;
        float topBound = maxPlayY - camVertExtent;

        Vector3 camPos = this._camera.transform.position;
        float camX = Mathf.Clamp(camPos.x, leftBound, rightBound);
        float camY = Mathf.Clamp(camPos.y, bottomBound, topBound);

        this._camera.transform.position = new Vector3(camX, camY, this._camera.transform.position.z);
    }

    /* ************* 
     * EDITOR
     ===============*/
    public void OnDrawGizmos() {
        Gizmos.color = Color.red;
        
        Vector3 topLeft = new Vector3(this.playArea.x, this.playArea.y, 0) + this.playAreaOffset;
        Vector3 topRight = new Vector3(this.playArea.width, this.playArea.y, 0) + this.playAreaOffset;

        Vector3 bottomnLeft = new Vector3(this.playArea.x, this.playArea.height, 0) + this.playAreaOffset;
        Vector3 bottomRight = new Vector3(this.playArea.width, this.playArea.height, 0) + this.playAreaOffset;

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomnLeft);
        Gizmos.DrawLine(bottomnLeft, topLeft);
    }
}
