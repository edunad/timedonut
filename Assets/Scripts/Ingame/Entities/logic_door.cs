using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logic_door : MonoBehaviour {

    [Header("Door settings")]
    public Vector2 offset;
    public float doorSpeed;

    private Vector3 _originalPos;

	public void Awake () {
        this._originalPos = this.transform.position;

    }
}
