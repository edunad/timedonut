
using UnityEngine;

public class logic_particle : MonoBehaviour {
    public float deathSeconds = 2f;

    [HideInInspector]
    public bool canKill;
    private float spawnTime;

    public void Awake() {
        this.tag = "particle_object";

        this.spawnTime = Time.time + deathSeconds;
        this.canKill = true;
    }

    public void Update() {
        if (!this.canKill || Time.time < this.spawnTime) return;
        Destroy(this.gameObject); // Todo : Shrink / Fade effect?
    }
}
