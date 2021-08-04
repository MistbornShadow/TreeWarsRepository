using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TW.NetworkBehavior;

public class SearchingScript : MonoBehaviour
{
    void Start(){
        //create server list
        if(!WebSocketScript.joined) WebSocketScript.generateWebsocket();
        WebSocketScript.SendServerListRequest();
        WebSocketScript.ws.OnMessage += (sender, e) => {
            string s = e.Data;
            WebSocketScript.recieveMessage(s);
        };
    }
}
