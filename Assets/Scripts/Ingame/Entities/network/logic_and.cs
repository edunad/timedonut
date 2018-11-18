using System;
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

        // Store all recieved data
        this._networkData = new Dictionary<string, network_data>();
    }

    /* ************* 
     * EVENTS + TIME
     ===============*/
    public void OnEnable() {
        CoreController.OnTimeChange += this.setTimeStatus;
    }

    public void OnDisable() {
        CoreController.OnTimeChange -= this.setTimeStatus;
    }

    public void setTimeStatus(bool running) {
        this._networkData.Clear(); // Clear all recieved data

        this._animator.SetInteger("status", 0);
        this.setCableColor(Color.red);
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
            this._networkData.Add(id, new network_data() { sender = sender, data = data }); // Create a new network data
        } else {
            this._networkData[id].data = data; // Update existing data
        }

        this.updateStatus();
    }
    

    private void updateStatus() {
        if (this._networkData.Count < this._maxSenders) {
            this.alertLogic(false);
            return;
        }

        List<string> keys = this._networkData.Keys.ToList();

        // "AND" logic
        this.alertLogic(this._networkData[keys[0]].data == 1 && this._networkData[keys[1]].data == 1);
    }

    private void setCableColor(Color col) {
        if (this._cable == null) return;
        this._cable.setCableColor(col);
    }

    private void alertLogic(bool isEnabled) {
        if (this.reciever == null) return;

        this.setCableColor(isEnabled ? Color.green: Color.red);
        this._animator.SetInteger("status", isEnabled ? 1 : 0);
        this.reciever.BroadcastMessage("onDataRecieved", new object[] { this.gameObject, isEnabled }, SendMessageOptions.DontRequireReceiver);
    }
}
