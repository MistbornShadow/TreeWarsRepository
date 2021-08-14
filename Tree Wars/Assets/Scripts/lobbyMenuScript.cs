using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TW.NetworkBehavior;
using WebSocketSharp;
using TW.PlayerLobby;

public class lobbyMenuScript : MonoBehaviour
{
    public Text gameIDText;
    public PlayerLobbyController lobby;

    public GameObject host;
    public GameObject guest;

    private float time;

    //positions for the player objects upon clicking on the team indicated
    public GameObject AutumnTeamPosition;
    public GameObject WinterTeamPosition;
    public GameObject Null1Position;
    public GameObject Null2Position;

    public int gameID;
    void Start(){
        Debug.Log("Start");
        addGameID();
        addTitle();
        WebSocketScript.ws.OnMessage += (sender, e) => {
            string s = e.Data;
            WebSocketScript.recieveMessage(s);
        };
    }

    void Awake(){
        Debug.Log("Awake");
        time = 0.0f;
    }

    void Update(){
        time += Time.deltaTime;
        if (time > 10.0) {
            Debug.Log("Update: GuestID " + WebSocketScript.guestID + " PlayerLobby guest " + 
        PlayerLobby.guest + " joined " + WebSocketScript.joined);
            Debug.Log("Websocket team roster object: " + WebSocketScript.ts.player1 + " " + WebSocketScript.ts.player2 + 
            " " + WebSocketScript.ts.autumn + " " + WebSocketScript.ts.winter); 
        time = 0.0f;
        if(WebSocketScript.guestID != -1 && PlayerLobby.guest == -1){
            Debug.Log("Added Guest");
            lobby.addGuestID(WebSocketScript.guestID);
        }
        if(WebSocketScript.joined){
            Debug.Log("Entered joined if");
            updatePlayerConditions(WebSocketScript.ts);
        }
        }
    }

    void OnApplicationQuit() {
        WebSocketScript.gameExit();
    }

    public void updatePlayerConditions(TeamsState ts){
        Debug.Log("updatePlayerConditions Activated");
        if(ts.autumn == -1 && ts.winter == -1){
            host.transform.position = Null1Position.transform.position;
            guest.transform.position = Null2Position.transform.position;
        }
        else if(ts.autumn != -1 && ts.winter == -1) {
            Debug.Log("ts.autumn != -1");
            //change position to team chosen
            if(ts.autumn == PlayerLobby.host){
                host.transform.position = AutumnTeamPosition.transform.position;
                guest.transform.position = Null2Position.transform.position;
            }
            else {
                guest.transform.position = AutumnTeamPosition.transform.position;
                host.transform.position = Null1Position.transform.position;
            }
        }
        else if(ts.winter != -1 && ts.autumn == -1) {
            Debug.Log("ts.winter != -1");
            //change position to team chosen
            if(ts.winter == PlayerLobby.host){
                host.transform.position = WinterTeamPosition.transform.position;
                guest.transform.position = Null2Position.transform.position;
            }
            else {
                guest.transform.position = WinterTeamPosition.transform.position;
                host.transform.position = Null1Position.transform.position;
            }
        }
        else{
            if(ts.autumn == PlayerLobby.host){
                host.transform.position = AutumnTeamPosition.transform.position;
                guest.transform.position = WinterTeamPosition.transform.position;
            }
            else {
                guest.transform.position = AutumnTeamPosition.transform.position;
                host.transform.position = WinterTeamPosition.transform.position;
            }
        }        
    }

    public void addGameID(){
        gameIDText.text = "Game ID: " + WebSocketScript.gameID;
    }

    public void nullifyTeamSelection(){
        WebSocketScript.updateTeamCondition(0);
    }
    public void selectTeamAutumn(){
        WebSocketScript.updateTeamCondition(1);
    }

    public void selectTeamWinter(){
        WebSocketScript.updateTeamCondition(2);
    }

    public void addTitle(){
        if(WebSocketScript.title == 1){
            lobby.addHostID(WebSocketScript.playerID);
        }
        else{
            lobby.addGuestID(WebSocketScript.playerID);
            WebSocketScript.requestHostID(WebSocketScript.gameID);
            lobby.addHostID(WebSocketScript.hostID);
        }
    }
}