
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class logic_nailer : MonoBehaviour {

    public GameObject nailInstance;
    public int shootCount;

    private List<Collider2D> _colliders;
    private List<GameObject> _nails;
    private List<string> _allowedColliders;
    private AudioClip[] _audioClips;

    private AudioSource _audioSource;

    private bool _timeRunning = false;
    private bool _isTimed = false;
    private bool _hasWon = false;

    public void Awake() {
        this._colliders = new List<Collider2D>();
        this._nails = new List<GameObject>();

        this._isTimed = this.GetComponent<logic_time>() != null;
        this._allowedColliders = new List<string>() {
            "paradox_object",
            "timed_object"
        };

        #region Sound Loading
        this._audioSource = this.GetComponent<AudioSource>();
        this._audioSource.playOnAwake = false;
        this._audioSource.volume = 0.13f;
        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Nailgun/nailgun_shoot_1"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Nailgun/nailgun_shoot_2"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Nailgun/nailgun_shoot_empty"),
        };
        #endregion
    }

    /* ************* 
     * COLLISION
     ===============*/
    public void OnTriggerEnter2D(Collider2D collider) {
        if (this._hasWon || !this._timeRunning) return;

        if (!this._allowedColliders.Contains(collider.tag)) return;
        if (this._colliders.Count > 0 || this._colliders.Contains(collider)) return;

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
    private void onWin() {
        this._hasWon = true;
    }

    public void OnEnable() {
        CoreController.OnTimeChange += this.setTimeStatus;
        CoreController.OnGameWin += this.onWin;
    }

    public void OnDisable() {
        CoreController.OnTimeChange -= this.setTimeStatus;
        CoreController.OnGameWin += this.onWin;
    }

    private void setTimeStatus(bool enabled) {
        if (this._hasWon) return;

        this.cleanNails(); // Cleanup the nails
        this._timeRunning = enabled;
    }

    /* ************* 
     * SHOOTING
     ===============*/
    private void cleanNails() {
        if (this._nails.Count > 0) return;
        foreach (GameObject obj in this._nails)
            Destroy(obj);

        this._nails.Clear();
    }

    private void shootNail() {
        // Play empty "click" sound
        if (this._nails.Count >= shootCount) {
            this._audioSource.pitch = 1f;
            this._audioSource.clip = this._audioClips[this._audioClips.Length - 1];
            this._audioSource.Play();

            return;
        }

        GameObject nail = GameObject.Instantiate(nailInstance);
        nail.name = "nail_instance_" + this._nails.Count;

        Vector3 shootPos = this.transform.TransformPoint(Vector3.right * 0.2f) + new Vector3(0.1f, 0.1f, 0);
        nail.transform.position = new Vector3(shootPos.x, shootPos.y, this.transform.position.z);
        nail.transform.rotation = this.transform.rotation;
        nail.layer = this._isTimed ? 11 : 10;

        Rigidbody2D nailBody = nail.GetComponent<Rigidbody2D>();
        nailBody.AddForce(this.transform.right * 80, ForceMode2D.Impulse); // TODO : Fix
        nailBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Play shooting sound
        this._audioSource.clip = this._audioClips[Random.Range(0, this._audioClips.Length - 1)];
        this._audioSource.Play();
        this._audioSource.pitch = Random.Range(0.9f, 1.2f);

        this._nails.Add(nail);
    }
}
