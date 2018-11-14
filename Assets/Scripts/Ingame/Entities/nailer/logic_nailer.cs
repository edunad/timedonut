using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logic_nailer : MonoBehaviour {

    public GameObject nailInstance;
    public int shootCount;

    private List<Collider2D> _colliders;
    private List<GameObject> _nails;
    private bool _timeRunning = false;

    public void Awake() {
        this._colliders = new List<Collider2D>();
        this._nails = new List<GameObject>();
    }

    /* ************* 
     * COLLISION
     ===============*/
    public void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "particle_object") return;
        if (!this._timeRunning || this._colliders.Count > 0) return;

        if (this._colliders.Contains(collider)) return;
        this._colliders.Add(collider);

        this.shootNail();
    }

    public void OnTriggerExit2D(Collider2D collider) {
        if (!this._colliders.Contains(collider)) return;
        this._colliders.Remove(collider);
    }

    /* ************* 
     * EVENTS + TIME
     ===============*/
    public void OnEnable() {
        TimeController.OnTimeChange += this.setTimeStatus;
    }

    public void OnDisable() {
        TimeController.OnTimeChange -= this.setTimeStatus;
    }

    private void setTimeStatus(bool enabled) {
        if (!enabled && this._nails.Count > 0) {
            foreach (GameObject obj in this._nails)
                Destroy(obj);

            this._nails.Clear();
        }

        this._timeRunning = enabled;
    }

    /* ************* 
     * SHOOTING
     ===============*/
    private void shootNail() {
        if (this._nails.Count >= shootCount) return; // Todo play empty nail sound

        GameObject nail = GameObject.Instantiate(nailInstance);
        nail.name = "nail_instance_" + this._nails.Count;

        Vector3 shootPos = this.transform.TransformPoint(Vector3.right * 0.2f) + new Vector3(0.1f, 0.1f, 0);
        nail.transform.position = new Vector3(shootPos.x, shootPos.y, this.transform.position.z);
        nail.transform.rotation = this.transform.rotation;
        nail.layer = 11;

        Rigidbody2D nailBody = nail.GetComponent<Rigidbody2D>();
        nailBody.AddForce(this.transform.right * 80, ForceMode2D.Impulse);
        nailBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        this._nails.Add(nail);
    }
}
