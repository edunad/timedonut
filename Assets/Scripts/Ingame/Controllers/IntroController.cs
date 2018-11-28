
using Kino;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour {

    public ui_timedtext _timeText;
    
    private AnalogGlitch _glichEffect;
    private PostProcessVolume _processVolume;
    private bool _endIntro;

    public void Awake() {
        this._processVolume = GameObject.Find("Camera").GetComponent<PostProcessVolume>();
        this._processVolume.profile.TryGetSettings(out _glichEffect);
        this._glichEffect.scanLineJitter.value = 0.107f;
    }

    public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        this._timeText.startText(() => {
            Debug.Log("text done");
            this._endIntro = true;
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
        if (this._endIntro) {
            if (this._glichEffect.scanLineJitter.value < 1) {
                this._glichEffect.scanLineJitter.value += 0.02f;
            } else {
                this._endIntro = false;
                this.loadNextLevel();
            }

        }

        util_timer.Update();
    }

    public void OnDestroy() {
        util_timer.Clear();
    }

    private void loadNextLevel() {
        int sceneID = SceneManager.GetActiveScene().buildIndex;

        PlayerPrefs.SetInt("lvl-" + sceneID, 5);
        PlayerPrefs.Save();

        PlayerPrefs.SetInt("loading_scene_index", sceneID + 1);
        SceneManager.LoadScene("level-loader", LoadSceneMode.Single);
    }
}
