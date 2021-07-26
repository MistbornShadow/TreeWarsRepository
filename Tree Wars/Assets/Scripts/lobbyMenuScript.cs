using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TW.Player;
using TW.NetworkBehavior;
using WebSocketSharp;

public class lobbyMenuScript : MonoBehaviour
{
    public Text gameIDText;

    public GameObject host;
    public int hostID;
    public GameObject guest;
    public int guestID;

    public int gameID;
    void Start(){
        addGameID();
        WebSocketScript.ws.OnMessage += (sender, e) => {
            string s = e.Data;
            WebSocketScript.recieveMessage(s);
        };
    }

    public void addGameID(){
        gameIDText.text = "Game ID: " + WebSocketScript.gameID;
    }
}