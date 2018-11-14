using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class network_data {
    public GameObject sender;
    public int data;
}

public class logic_and : MonoBehaviour {
    public GameObject reciever;

    private Dictionary<string, network_data> networkData;
    private int maxSenders = 2;

    public void Awake() {
        this.networkData = new Dictionary<string, network_data>();
    }

    /* ************* 
     * EVENTS + TIME
     ===============*/
    public void OnEnable() {
        TimeController.OnTimeChange += this.setTimeStatus;
    }

    public void OnDisable() {
        TimeController.OnTimeChange -= this.setTimeStatus;
    }

    public void setTimeStatus(bool running) {
        if (!running) this.networkData.Clear();
    }

    /* ************* 
     * LOGIC
     ===============*/
    public void onDataRecieved(object[] msg) {
        if (msg == null || msg.Length < 1) return;

        GameObject sender = msg[0] as GameObject;
        if (sender == null) return;
        int data = Convert.ToInt32(msg[1]);

        string id = sender.GetInstanceID().ToString();

        if (!this.networkData.ContainsKey(id.ToString())) {
            if (this.networkData.Count > maxSenders) return;
            this.networkData.Add(id, new network_data() { sender = sender, data = data });
            this.updateStatus();
        } else {
            this.networkData[id].data = data; // Update data
            this.updateStatus();
        }
    }
    

    private void updateStatus() {
        if (this.networkData.Count < maxSenders) {
            this.alertLogic(false);
            return;
        }

        List<string> keys = this.networkData.Keys.ToList();
        this.alertLogic(this.networkData[keys[0]].data == 1 && this.networkData[keys[1]].data == 1);
    }

    private void alertLogic(bool isEnabled) {
        if (this.reciever == null) return;
        this.reciever.BroadcastMessage("onDataRecieved", new object[] { this.gameObject, isEnabled }, SendMessageOptions.DontRequireReceiver);
    }
}
