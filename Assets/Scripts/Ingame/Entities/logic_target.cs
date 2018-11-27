
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class logic_target : MonoBehaviour {
    [Header("Target settings")]
    public Color shirtColor = new Color(0.31f, 0.34f, 0.48f, 1f);
    public Color pantsColor = new Color(0.51f, 0.54f, 0.69f, 1f);

	private CoreController _core;
	private BoxCollider2D _boxCollider;
    private Animator _animator;
    private Material _material;
    private SpriteRenderer _renderer;

    private bool _timeRunning;
    private bool _isDisabled = false;

	// Use this for initialization
	public void Awake () {
        this._material = new Material(Shader.Find("Char_shader"));

        this._renderer = GetComponent<SpriteRenderer>();
        this._renderer.sharedMaterial = this._material;

        this._animator = GetComponent<Animator>();
        this._animator.speed = 0f;

        this._boxCollider = GetComponent<BoxCollider2D>();
		this._boxCollider.isTrigger = true;

		this._core = GameObject.Find("Core").GetComponent<CoreController>();

        this.name = "logic_target";
        this.gameObject.layer = 12;

        this.updateColors();
    }

    /* ************* 
     * DEATH
    ===============*/
    // Update is called once per frame
    public void OnTriggerEnter2D(Collider2D collision) {
		if (collision == null || !this._timeRunning || this._isDisabled) return;
        this.killPlayer();
    }

    public void killPlayer() {
        Debug.Log("DEATH : <color='red'>" + this._core.currentTime + "</color>"); // For death_time

        this._core.onTargetDeath();
        this._isDisabled = true;
    }

    /* ************* 
     * SHADER
    ===============*/
    private void updateColors() {
        this._material.SetColor("_shirtColor", this.shirtColor);
        this._material.SetColor("_pantsColor", this.pantsColor);
    }

    /* ************* 
	 * EVENTS + TIME
	===============*/
    private void onWin() {
		this._isDisabled = true; // Disable the script
	}

	public void OnEnable() {
		CoreController.OnGameWin += this.onWin;
		CoreController.OnTimeChange += this.setTimeStatus;
	}

	public void OnDisable() {
		CoreController.OnGameWin -= this.onWin;
		CoreController.OnTimeChange -= this.setTimeStatus;
	}

	private void setTimeStatus(bool started) {
		this._timeRunning = started;
        this._isDisabled = false;

        if (!started) this._animator.Rebind(); // Restart animation
        this._animator.speed = started ? 1f : 0f;
    }
}
