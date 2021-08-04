using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TW.NetworkBehavior;
using WebSocketSharp;

public class lobbyMenuScript : MonoBehaviour
{
    public Text gameIDText;
    PlayerLobby lobby;

    public GameObject host;
    public GameObject guest;

    public int gameID;
    void Start(){
        addGameID();
        addTitle();
        WebSocketScript.ws.OnMessage += (sender, e) => {
            string s = e.Data;
            WebSocketScript.recieveMessage(s);
        };
    }

    public void addGameID(){
        gameIDText.text = "Game ID: " + WebSocketScript.gameID;
    }

    public void addTitle(){
        if(WebSocketScript.title = 1){
            lobby.host = WebSocketScript.playerID;
        }
        else{
            lobby.guest = WebSocketScript.playerID;
            WebSocketScript.requestHostID();
            lobby.host = WebSocketScript.hostID;
        }
    }
}