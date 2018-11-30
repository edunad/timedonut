
using UnityEngine;

public class util_sfx: MonoBehaviour {
    private AudioSource _audio;
    private GameObject _audioObject;

	public void Awake () {
        // Create a new temp object to prevent it from using an existing audiosource
        this._audioObject = new GameObject();
        this._audioObject.transform.parent = this.transform;
        this._audioObject.name = "temp_audioobj";

        this._audio = this._audioObject.AddComponent<AudioSource>();
        this._audio.playOnAwake = false;
	}

    public void playClip(string source, float volume) {
        if (this._audio == null) return;

        AudioClip clip = AssetsController.GetResource<AudioClip>("Sounds/" + source);
        if (clip == null) throw new UnityException("Failed to find clip {" + source + "}");

        this._audio.volume = Mathf.Clamp(OptionsController.effectsVolume / 1f * volume, 0f, 1f);
        this._audio.clip = clip;
        this._audio.Play();
    }
}
