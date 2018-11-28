using Assets.Scripts.models;
using UnityEngine;

public class logic_constant : MonoBehaviour {
    [Header("Constant settings")]
    public string networkHeader = "active";
    public int networkValue = 0;
    public GameObject reciever;

    /* ************* 
     * EVENTS + TIME
     ===============*/
    private void setTimeStatus(bool enabled) {
        if (!enabled) return;
        this.sendNetwork();
    }

    public void OnEnable() {
        CoreController.OnTimeChange += this.setTimeStatus;
    }

    public void OnDisable() {
        CoreController.OnTimeChange -= this.setTimeStatus;
    }

    /* ************* 
     * LOGIC
     ===============*/
    private void sendNetwork() {
        if (this.reciever == null) return;

        this.reciever.BroadcastMessage("onDataRecieved",
            new network_data() {
                sender = this.gameObject,
                header = this.networkHeader,
                data = this.networkValue
            },
            SendMessageOptions.DontRequireReceiver);
    }
}
