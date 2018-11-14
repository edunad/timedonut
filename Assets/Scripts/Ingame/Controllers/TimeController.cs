
using System.Collections.Generic;
using UnityEngine;

public class TimeController : ScriptableObject {
    public delegate void onTimeChange(bool start);
    public static event onTimeChange OnTimeChange;

    public bool timeRunning;
    
    public void timeStatus(bool start) {
        this.timeRunning = start;

        CoreController.AntiParaController.setVisibility(start);
        if (OnTimeChange != null) OnTimeChange(start);
    }
}
