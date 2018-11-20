using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class ui_intro : MonoBehaviour {
    private SpriteRenderer _renderer;
    private Vector3 _fadeStart, _fadeEnd;
    private AudioSource _audioSource;

    private AudioClip[] _audioClips;

    private bool _isFading;
    private float _journeyLength;
    private float _startTime;
    private Action _onFadeComplete;

    public float fadeSpeed;

    public void Awake() {
        this.name = "ui_intro";

        this._renderer = GetComponent<SpriteRenderer>();

        this._audioSource = GetComponent<AudioSource>();
        this._audioSource.playOnAwake = false;
        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/UI/intro_in"),
            AssetsController.GetResource<AudioClip>("Sounds/UI/intro_out")
        };
    }

    public void Start() {
        if (!Application.isPlaying) this._renderer.enabled = false;
        else this._renderer.enabled = true;
    }

    public void triggerFade(bool fadeIn, Vector3 fadeStart, Vector3 fadeEnd, Action onComplete = null) {
        if (this._isFading) return;

        this._fadeStart = fadeStart;
        this._fadeEnd = fadeEnd;

        this._renderer.flipX = fadeIn;

        this.transform.localPosition = fadeStart;

        this._journeyLength = Vector3.Distance(fadeStart, fadeEnd);
        this._startTime = Time.time;

        this._onFadeComplete = onComplete;
        this._isFading = true;

        if (fadeIn) this._audioSource.clip = this._audioClips[0];
        else this._audioSource.clip = this._audioClips[1];

        this._audioSource.Play();
    }

    // Update is called once per frame
    public void Update() {
        if (!this._isFading) return;

        float distCovered = (Time.time - this._startTime) * this.fadeSpeed;
        float fracJourney = distCovered / _journeyLength;

        this.transform.localPosition = Vector3.Lerp(this._fadeStart, this._fadeEnd, fracJourney);

        if (fracJourney >= 1) {
            this._isFading = false;
            if (_onFadeComplete != null)
                this._onFadeComplete();
        }

    }

    public void OnDrawGizmos() {
        if (this._renderer == null) return;
        Gizmos.color = Color.black;

        Bounds bsprite = this._renderer.bounds;
        Gizmos.DrawWireCube(bsprite.center, bsprite.size);
    }
}
