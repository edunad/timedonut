using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class logic_target : MonoBehaviour {
    private CoreController _core;
    private BoxCollider2D _boxCollider;

	// Use this for initialization
	public void Start () {
		this.name = "logic_target";
        this.gameObject.layer = 12;

        this._boxCollider = GetComponent<BoxCollider2D>();
        this._boxCollider.isTrigger = true;

        this._core = GameObject.Find("Core").GetComponent<CoreController>();
	}

    // Update is called once per frame
    public void OnTriggerEnter2D(Collider2D collision) {
        if (collision == null) return;
        Debug.Log("DEATH : <color='red'>" + this._core.currentTime + "</color>");
    }

    public void OnDrawGizmos() {
		Gizmos.color = new Color(255, 0, 0, 100);

		// Draw PIT
		Vector3 col = transform.position;

		// Draw Icon
		Gizmos.DrawIcon(new Vector3(col.x, col.y, col.z), "logic_dead", true);
	}
}
