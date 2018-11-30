using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour {

    public GameObject loading;

    private int _loadIndex;
    private AsyncOperation _asyncOp;

	public void Start () {
        this._loadIndex = PlayerPrefs.GetInt("loading_scene_index", -1); // There is probably a better way ¯\_(ツ)_/¯
        if (this._loadIndex == -1) throw new UnityException("Invalid scene index");

        // Should show load?
        bool hideLoad = (PlayerPrefs.GetInt("loading_noLoad", 0) == 1);
        this.loading.SetActive(!hideLoad);

        StartCoroutine(this.startLoading());
	}

    private void Update() {
        util_timer.Update();
    }

    private IEnumerator startLoading() {
        this._asyncOp = SceneManager.LoadSceneAsync(this._loadIndex, LoadSceneMode.Single);
        this._asyncOp.allowSceneActivation = false;

        while (!this._asyncOp.isDone) {
            if (this._asyncOp.progress == 0.9f) {
                util_timer.Simple(1f, () => { // Small delay
                    this._asyncOp.allowSceneActivation = true;
                    PlayerPrefs.SetInt("loading_scene_index", -1); // Reset
                    PlayerPrefs.SetInt("loading_noLoad", 0);
                });
            }

            yield return null;
        }
    }
}
