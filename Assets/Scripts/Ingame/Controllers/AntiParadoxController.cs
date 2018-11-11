
using UnityEngine;

public class AntiParadoxController : ScriptableObject {

    public bool isVisible;

    private logic_antiparadox[] _antiParadoxObjects;

    public void init() {
        this._antiParadoxObjects = Object.FindObjectsOfType<logic_antiparadox>();
    }

    public void setVisibility(bool visible) {
        if (this._antiParadoxObjects.Length <= 0) return;
        for (int i = 0; i < this._antiParadoxObjects.Length; i++) {
            if (this._antiParadoxObjects[i] == null) continue;
            this._antiParadoxObjects[i].setParadoxVisibility(visible);
        }

        this.isVisible = visible;
    }
}
