using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ui_spinning : MonoBehaviour {

    public float spinSpeed;
	// Update is called once per frame
	void Update () {
		this.transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * spinSpeed);
    }
}
