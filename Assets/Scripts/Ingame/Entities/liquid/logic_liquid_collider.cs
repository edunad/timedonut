using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class logic_liquid_collider : MonoBehaviour {

    private BoxCollider2D _boxCollider;
    public logic_liquid _controller;
    public int indx;

    public void Awake() {
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.isTrigger = true;
        _boxCollider.offset -= new Vector2(0, -0.20f);
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (_controller == null) return;
        if (other.attachedRigidbody == null) return;

        Vector3 pos = other.transform.position - new Vector3(0f, _boxCollider.bounds.size.y + 0.5f, 0f);
        _controller.Splash(indx, pos, other.attachedRigidbody.velocity.y / other.attachedRigidbody.mass, other.gameObject);
    }
}