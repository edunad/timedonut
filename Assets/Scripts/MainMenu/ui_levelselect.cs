using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ui_levelselect : MonoBehaviour {

    [Header("Level select settings")]
    public bool hasNoRating;
    public int levelIndex;

    [Header("Level select objects")]
    public TextMesh playerMoves;
    public GameObject lockedGameobject;

	public void Awake () {
        this.setupMovesText();
        this.setupLock();
    }

    private void setupMovesText() {
        if (this.hasNoRating) {
            this.playerMoves.text = ""; // Probably intro ?
        } else {
            bool locked = this.isLevelLocked(this.levelIndex);
            this.playerMoves.text = (locked ? "?" : this.getLevelScore(this.levelIndex).ToString()) + "★";
        }
    }

    private void setupLock() {
        if (this.hasNoRating) return;
        this.lockedGameobject.SetActive(this.isLevelLocked(this.levelIndex));
    }
    
    /* ************* 
     * LEVEL LOADING
    ===============*/
    public void loadLevel(int indx) {
        PlayerPrefs.SetInt("loading_noLoad", 0);
        PlayerPrefs.SetInt("loading_scene_index", indx);
        SceneManager.LoadScene("level-loader", LoadSceneMode.Single);
    }

    private int getLevelScore(int indx) {
        return PlayerPrefs.GetInt("lvl-" + indx, -1);
    }

    private bool isLevelLocked(int indx) {
        if (this.hasNoRating) return false;
        return PlayerPrefs.GetInt("lvl-" + indx, -1) < 0;
    }

    public void OnUIClick(string elementID) {
        if (elementID != "lvl-button" || this.isLevelLocked(this.levelIndex)) return;
        this.loadLevel(this.levelIndex);
    }
}
