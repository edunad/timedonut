
using UnityEngine;

public class util_material : MonoBehaviour {
    [HideInInspector]
	public Material material;

    public void setMaterial(Material mat) {
        this.material = mat;
        this.setupMaterial();
    }

    public void setMaterialColor(string shaderVar, Color color) {
        if (this.material == null) return;
        this.material.SetColor(shaderVar, color);
    }

    public void setMaterialFloat(string shaderVar, float value) {
        if (this.material == null) return;
        this.material.SetFloat(shaderVar, value);
    }

    /* ************* 
     * SETUP
     ===============*/
    private SpriteRenderer[] getRenderSprites() {
        return this.GetComponentsInChildren<SpriteRenderer>();
    }

    private void setupMaterial() {
        if (this.material == null) throw new UnityException("Invalid material");

        SpriteRenderer[] renderers = this.getRenderSprites();
        if (renderers == null || renderers.Length <= 0) return;

        foreach (SpriteRenderer spr in renderers) {
            if (spr == null) continue;
            spr.material = this.material;
        }
    }
}
