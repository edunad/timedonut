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

    private Animator _animator;
    private logic_cable _cable;

    private Dictionary<string, network_data> _networkData;
    private int _maxSenders = 2;

    public void Awake() {
        this._cable = GetComponent<logic_cable>();
        this._animator = GetComponent<Animator>();
        this._animator.SetInteger("status", 0);

        this._networkData = new Dictionary<string, network_data>();
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
        this._networkData.Clear();
        this._animator.SetInteger("status", 0);
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

        if (!this._networkData.ContainsKey(id.ToString())) {
            if (this._networkData.Count > this._maxSenders) return;
            this._networkData.Add(id, new network_data() { sender = sender, data = data });
            this.updateStatus();
        } else {
            this._networkData[id].data = data; // Update data
            this.updateStatus();
        }
    }
    

    private void updateStatus() {
        if (this._networkData.Count < this._maxSenders) {
            this.alertLogic(false);
            return;
        }

        List<string> keys = this._networkData.Keys.ToList();
        this.alertLogic(this._networkData[keys[0]].data == 1 && this._networkData[keys[1]].data == 1);
    }

    private void alertLogic(bool isEnabled) {
        if (this.reciever == null) return;

        if (this._cable != null) this._cable.setCableColor(isEnabled ? Color.green: Color.red);
        this._animator.SetInteger("status", isEnabled ? 1 : 0);
        this.reciever.BroadcastMessage("onDataRecieved", new object[] { this.gameObject, isEnabled }, SendMessageOptions.DontRequireReceiver);
    }
}
