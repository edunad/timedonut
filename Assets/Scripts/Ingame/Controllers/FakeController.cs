
using Kino;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class FakeController : MonoBehaviour {
    public GameObject bagel;
    public GameObject eatBtn;
    public ui_timedtext timeText;
    public ui_timedtext storyofmylifeText;
    public ui_timedtext leaveText;

    private AnalogGlitch _glichEffect;
    private PostProcessVolume _processVolume;
    private AudioSource _audio;
    private util_sfx _sfx;
    private Animator _bagelAnim;

    private bool _canLeave;
    private bool _isSadMusic;

    public void Awake() {
        this._bagelAnim = bagel.GetComponent<Animator>();
        this._bagelAnim.SetInteger("status", 0);

        this._audio = GetComponent<AudioSource>();
        this._audio.loop = true;
        this._audio.playOnAwake = false;
        this._audio.volume = Mathf.Clamp(OptionsController.musicVolume / 1f * 0.5f, 0f, 1f);
        this._audio.Play();

        this._processVolume = GameObject.Find("Camera").GetComponent<PostProcessVolume>();
        this._processVolume.profile.TryGetSettings(out _glichEffect);
        this._glichEffect.scanLineJitter.value = 0.107f;

        this._sfx = GetComponent<util_sfx>();

        // Preload assets! - Faster play
        AssetsController.GetResource<AudioClip>("Sounds/Ingame/Music/Comfortable Mystery 4");
        AssetsController.GetResource<AudioClip>("Sounds/Ingame/Music/dundundunnnnn");
        AssetsController.GetResource<AudioClip>("Sounds/Ingame/Effects/door_close");
        AssetsController.GetResource<AudioClip>("Sounds/Ingame/Effects/door_open");

        // Vars
        this._canLeave = false;
        this._isSadMusic = false;
        this.eatBtn.SetActive(false);
    }

    public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        this._audio.Play();

        this.timeText.startText(() => {
            Debug.Log("Normal text completed, start blalbalbal");

            this.storyofmylifeText.startText(() => {
                Debug.Log("This should never be called :S");
            }, (string type) => {
                if (type == "sadmusic" && !this._isSadMusic) {
                    this._isSadMusic = true;

                    // Change track
                    this._audio.clip = AssetsController.GetResource<AudioClip>("Sounds/Ingame/Music/Comfortable Mystery 4");
                    this._audio.volume = Mathf.Clamp(OptionsController.musicVolume / 1f * 0.4f, 0f, 1f);
                    this._audio.Play();
                } else if (type == "leave") {
                    this._canLeave = true;
                    this.eatBtn.SetActive(true);
                }
            });
        },(string type) => {
            if (type == "reveal") {
                this._bagelAnim.SetInteger("status", 1);
            } else if (type == "dundundun") {
                this._sfx.playClip("Ingame/Music/dundundunnnnn", 0.3f);
            }
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

    /* ************* 
    * UI
    ===============*/
    public void OnUIClick(string element) {
        if (!this._canLeave || element != "UI_LEAVE_BTN") return;

        this._canLeave = false;
        this.eatBtn.SetActive(false);

        this._sfx.playClip("Ingame/Effects/door_open", 0.8f);
        // Play leave sounds

        util_timer.Simple(2f, () => {
            this.storyofmylifeText.stopText();

            this._audio.Stop();
            this.leaveText.startText(() => {
                int sceneID = SceneManager.GetActiveScene().buildIndex;
                PlayerPrefs.SetInt("loading_noLoad", 1);
                PlayerPrefs.SetInt("loading_scene_index", sceneID + 1);

                SceneManager.LoadScene("level-loader", LoadSceneMode.Single);
            }, (string type) => {
                if (type != "closedoor") return;
                this._sfx.playClip("Ingame/Effects/door_close", 0.8f);
            });
        });
    }
}
