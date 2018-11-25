
using UnityEngine;

public class ui_floating : MonoBehaviour {
	[Header("Time")]
	public float timeOffset = 0f;

	[Header("Timing Y")]
	public float ySpeed = 2f;
	public float yOffset = 0.0005f;

	[Header("Timing X")]
	public float xSpeed = 2f;
	public float xOffset = 0f;

	[HideInInspector]
	public bool isEnabled;

	private Vector3 _originalPos;

	public void Awake () {
		this.isEnabled = true;
	}
	
	public void Update () {
		if (!this.isEnabled) return;

		Vector3 pos = this.transform.localPosition;

		float y = Mathf.Sin((Time.time + timeOffset) * this.ySpeed) * this.yOffset;
		float x = Mathf.Cos((Time.time + timeOffset) * this.xSpeed) * this.xOffset;

        this.transform.localPosition = new Vector3(pos.x + x, pos.y + y, pos.z);
	}
}
