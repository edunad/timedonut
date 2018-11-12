using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logic_nailer : MonoBehaviour {
    public GameObject nailInstance;
    public int shootCount;

    private int shoots;

    public void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag != "paradox_object" && collider.tag != "timed_object") return;
        this.shootNail();
    }

    private void shootNail() {
        if (shoots >= shootCount) return;
        shoots++;

        GameObject nail = GameObject.Instantiate(nailInstance);
        nail.name = "nail_instance_" + shoots;
        nail.transform.position = this.transform.TransformPoint(Vector3.right * 0.2f) + new Vector3(0.1f, 0.1f, 0);
        nail.transform.rotation = this.transform.rotation;
        nail.layer = 11;

        Rigidbody2D nailBody = nail.GetComponent<Rigidbody2D>();
        nailBody.AddForce(this.transform.right * 80, ForceMode2D.Impulse);
    }
}
