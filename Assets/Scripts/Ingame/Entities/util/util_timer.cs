using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class util_timer {
    private static Dictionary<string, util_timer> _timers;
    private static int MxTimer = 0;

    private float _fCreationTime;
    private float _fNextIteration;

    public static Dictionary<string, util_timer> GetTimers {
        get { return _timers ?? (_timers = new Dictionary<string, util_timer>()); }
    }

    private float FElapsed {
        get { return (float)Time.time - _fCreationTime; }
    }

    private int Iterations { get; set; }
    private float Delay { get; set; }
    private Action Func { get; set; }
    private string UniqueName { get; set; }

    public static void Update() {
        try {
            if (_timers == null || _timers.Count < 0) {
                if (MxTimer > 0) MxTimer = 0; // Reset unique
                return;
            }

            for (int I = 0; I < _timers.Count; I++) {
                string name = _timers.Keys.ElementAt(I);
                if (_timers[name] != null)
                    _timers[name].UpdateTimer();
            }
        } catch (Exception err) {
            // TODO : ERROR SEND
            Debug.Log(err.StackTrace);
        }
    }

    public static util_timer Simple(float delay, Action func) {
        return Create(1, delay, func);
    }

    public static util_timer UniqueSimple(string name, float delay, Action func) {
        return Create(1, delay, func, name);
    }

    public static util_timer Create(int reps, float delay, Action func, string uniqueName = "") {
        if (String.IsNullOrEmpty(uniqueName)) {
            uniqueName = MxTimer.ToString();
            MxTimer++;
        }

        var t = new util_timer { Iterations = reps, Delay = delay, Func = func, UniqueName = uniqueName };

        // Check if timer already exists
        if (GetTimers.ContainsKey(uniqueName))
            if (GetTimers[uniqueName] != null)
                t = GetTimers[uniqueName];

        t.Start();
        return t;
    }

    public void Start() {
        if (!GetTimers.ContainsKey(this.UniqueName))
            GetTimers.Add(this.UniqueName, this);

        _fCreationTime = (float)Time.time;
        _fNextIteration = _fCreationTime + Delay;
    }

    public void Stop() {
        if (GetTimers.ContainsKey(this.UniqueName))
            GetTimers.Remove(this.UniqueName);
    }

    private void UpdateTimer() {
        var t = (float)Time.time;
        if (t >= _fNextIteration) {
            if (Func != null)
                Func();
            _fNextIteration = t + Delay;
            if (Iterations > 0) {
                Iterations--;
                if (Iterations == 0) Stop();
            }
        }
    }

    public static void Clear() {
        foreach (util_timer timer in GetTimers.Values.ToList())
            if (timer != null)
                timer.Stop();

        GetTimers.Clear();
    }
}
