using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour {

    private int _loadIndex;
    private AsyncOperation _asyncOp;

	public void Start () {
        this._loadIndex = PlayerPrefs.GetInt("loading_scene_index");
        if (this._loadIndex == -1) throw new UnityException("Invalid scene index");

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
                util_timer.Simple(2f, () => {
                    this._asyncOp.allowSceneActivation = true;
                    PlayerPrefs.SetInt("loading_scene_index", -1); // Reset
                });
            }

            yield return null;
        }
    }
}
