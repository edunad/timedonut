
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditController : MonoBehaviour {
    private AudioSource _audio;

    public void Awake() {
        this._audio = GetComponent<AudioSource>();
        this._audio.loop = true;
        this._audio.playOnAwake = false;
        this._audio.volume = Mathf.Clamp(OptionsController.musicVolume / 1f * 0.5f, 0f, 1f);

        this._audio.Play();
    }

    /* ************* 
    * UI
    ===============*/
    public void OnUIClick(string element) {
        if (element != "UI_LEAVE_BTN") return;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
