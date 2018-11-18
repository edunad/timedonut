using Assets.Scripts.models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class logic_and : MonoBehaviour {
    [SerializeField]
    public List<GameObject> recievers = new List<GameObject>();

    [Header("Network settings")]
    public string recieveHeader = "active";
    public string dataHeader = "active";

    // Vars
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
    public void onDataRecieved(network_data msg) {
        if (msg == null) return;
        if (msg.header != this.recieveHeader) return;

        string id = msg.sender.GetInstanceID().ToString();
        if (!this._networkData.ContainsKey(id)) {
            if (this._networkData.Count > this._maxSenders) return;
            this._networkData.Add(id, msg);
        } else {
            this._networkData[id] = msg; // Update existing data
        }

        // Update current status
        this.logicUpdate();
    }
    

    private void logicUpdate() {
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
        if (this.recievers == null || this.recievers.Count <= 0) return;

        this.setCableColor(isEnabled ? Color.green: Color.red);
        this._animator.SetInteger("status", isEnabled ? 1 : 0);

        // Broadcast
        foreach (GameObject obj in recievers) {
            if (obj == null) continue;

            obj.BroadcastMessage("onDataRecieved",
                new network_data() {
                    sender = this.gameObject,
                    header = this.dataHeader,
                    data = isEnabled ? 1 : 0
                }, SendMessageOptions.DontRequireReceiver);
        }
    }
}
