using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class logic_target : MonoBehaviour {
	private BoxCollider2D _boxCollider;

	// Use this for initialization
	public void Start () {
		this.name = "logic_target";

		_boxCollider = GetComponent<BoxCollider2D>();
		_boxCollider.isTrigger = true;

        this.gameObject.layer = 12;

	}

    // Update is called once per frame
    public void OnTriggerEnter2D(Collider2D collision) {
        if (collision == null) return;
        Debug.Log("DEAD");
    }

    public void OnDrawGizmos() {
		Gizmos.color = new Color(255, 0, 0, 100);

		// Draw PIT
		Vector3 col = transform.position;

		// Draw Icon
		Gizmos.DrawIcon(new Vector3(col.x, col.y, col.z), "logic_dead", true);
	}
}
