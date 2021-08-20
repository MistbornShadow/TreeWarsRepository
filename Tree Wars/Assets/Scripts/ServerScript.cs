using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TW.NetworkBehavior;

public class ServerScript : MonoBehaviour
{  
    [SerializeField]
    private Text text;

    public int gameID;

    public void SetID(string s){
        text.text = s;
    }

    public void setInt(int i){
        gameID = i;
    }

    public void sendJoin(){
        WebSocketScript.SendJoinRequest(gameID);
        WebSocketScript.isServerCreated = true;
        WebSocketScript.serverList = new List<int>();
    }
}
