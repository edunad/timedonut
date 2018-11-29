
using Kino;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class FakeController : MonoBehaviour {

    public ui_timedtext _timeText;

    private AnalogGlitch _glichEffect;
    private PostProcessVolume _processVolume;
    private AudioSource _audio;
    private AudioSource _dundunAudio;

    public void Awake() {
        this._audio = GetComponent<AudioSource>();
        this._audio.loop = true;
        this._audio.playOnAwake = false;
        this._audio.volume = Mathf.Clamp(OptionsController.musicVolume / 1f * 0.7f, 0f, 1f);

        this._dundunAudio = GameObject.Find("sfx_dun").GetComponent<AudioSource>(); ;
        this._dundunAudio.loop = false;
        this._dundunAudio.playOnAwake = false;
        this._dundunAudio.volume = Mathf.Clamp(OptionsController.musicVolume / 1f * 0.4f, 0f, 1f);

        this._processVolume = GameObject.Find("Camera").GetComponent<PostProcessVolume>();
        this._processVolume.profile.TryGetSettings(out _glichEffect);
        this._glichEffect.scanLineJitter.value = 0.107f;
    }

    public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        this._audio.Play();
        this._timeText.startText(() => {

        },() => {
            this._dundunAudio.Play();
        });
    }

    /* ************* 
    * EVENTS + TIMER
    ===============*/
    public void OnEnable() {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    public void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    public void Update() {
        util_timer.Update();
    }

    public void OnDestroy() {
        util_timer.Clear();
    }
}
