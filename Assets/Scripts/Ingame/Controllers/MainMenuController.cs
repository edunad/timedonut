using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public void Awake() {
        
    }

    public void loadLevel(int indx) {
        PlayerPrefs.SetInt("loading_scene_index", indx);
        SceneManager.LoadScene("level-loader", LoadSceneMode.Single);
    }

    /* ************* 
     * BUTTONS
    ===============*/
    public void OnUIClick(string elementID) {
        if (elementID == "test_btn") {
            this.loadLevel(2); // 2 = level-1
        }
    }

}
