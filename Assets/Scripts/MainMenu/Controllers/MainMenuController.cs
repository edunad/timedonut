
using UnityEngine;

public class MainMenuController : MonoBehaviour {
    [Header("MainMenu panels")]
    public GameObject levelMenu;
    public GameObject optionsMenu;

    private AudioSource _audioSource;

    public void Awake() {
        this._audioSource = GetComponent<AudioSource>();
        this._audioSource.playOnAwake = false;
        this._audioSource.loop = true;
        this._audioSource.volume = Mathf.Clamp(OptionsController.musicVolume / 1f * 0.5f, 0f, 1f);
        this._audioSource.Play();

        // Hide panels by default
        this.optionsMenu.SetActive(false);
        this.levelMenu.SetActive(false);
    }
    
    /* ************* 
     * MUSIC STUFF
     ===============*/
    public void updateMusicVolume() {
        this._audioSource.volume = Mathf.Clamp(OptionsController.musicVolume / 1f * 0.5f, 0f, 1f);
    }

    /* ************* 
     * UI
    ===============*/
    public void displayOptions(bool display) {
        this.optionsMenu.SetActive(display);
    }

    public void displayLevelSelect(bool display) {
        this.levelMenu.SetActive(display);
    }
    
    /* ************* 
     * BUTTONS
    ===============*/
    public void OnUIClick(string elementID) {
        if (elementID == "UI_OPTIONS") {
            this.displayOptions(true);
            this.displayLevelSelect(false);
        } else if (elementID == "UI_OPT_OK_BTN") {
            this.displayOptions(false);
            this.displayLevelSelect(false);
        } else if (elementID == "UI_LVL") {
            this.displayLevelSelect(true);
            this.displayOptions(false);
        } else if (elementID == "UI_LVL_OK_BTN") {
            this.displayOptions(false);
            this.displayLevelSelect(false);
        } else if (elementID == "UI_QUIT") {
            Application.Quit();
        }
    }

}
