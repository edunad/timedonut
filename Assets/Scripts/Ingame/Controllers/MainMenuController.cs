
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public void loadLevel(int indx) {
        PlayerPrefs.SetInt("loading_scene_index", indx);
        SceneManager.LoadScene("level-loader", LoadSceneMode.Single);
    }

    private int getLevelScore(int indx) {
        return PlayerPrefs.GetInt("lvl-" + indx, -1);
    }

    private bool isLevelLocked(int indx) {
        int score = PlayerPrefs.GetInt("lvl-" + indx, -1);
        return score < 0;
    }

    /* ************* 
     * BUTTONS
    ===============*/
    public void OnUIClick(string elementID) {
        if (elementID == "test_btn") {
            this.loadLevel(2); // 2 = level-1
        }else if(elementID == "test_btn_lvl2") {
            this.loadLevel(5); // 2 = level-1
        }
    }

}
