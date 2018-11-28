
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class util_aspectRatio : MonoBehaviour {
    public float WIDTH = 1024f;
    public float HEIGHT = 768f;
    
    public void Awake () {
        Camera _cam = GetComponent<Camera>();
        _cam.aspect = WIDTH / HEIGHT;
	}
}
