
using TMPro;
using UnityEngine;

public class ui_counter : MonoBehaviour {

    [Header("Counter settings")]
    public int percentage = 0;

    private TextMeshProUGUI _text;

    public void Awake() {
        this._text = this.GetComponentInChildren<TextMeshProUGUI>();
        this.setPercentage(this.percentage);
    }

    public void setPercentage(int perc) {
        if (this._text == null) return;

        string tx = "";

        if (perc < 10) tx = perc + "  %";
        else if(perc >= 10 && perc < 100) tx = perc + " %";
        else if (perc >= 100) tx = perc + "%";

        this._text.SetText(tx);
    }
}
