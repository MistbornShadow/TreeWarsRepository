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
    public static PlayerLobbyController lobby;

    public GameObject host;
    public GameObject guest;

    //positions for the player objects upon clicking on the team indicated
    public GameObject AutumnTeamPosition;
    public GameObject WinterTeamPosition;
    public GameObject NullPosition;

    public int gameID;
    void Start(){
        addGameID();
        addTitle();
        WebSocketScript.ws.OnMessage += (sender, e) => {
            string s = e.Data;
            WebSocketScript.recieveMessage(s);
        };
    }

    void Update(){
        if(lobby.checkFull()){
            checkPlayerConditions(WebSocketScript.ts, PlayerLobby.host, PlayerLobby.guest);
            updatePlayerConditions(WebSocketScript.ts);
        }
    }

    public void checkPlayerConditions(TeamsState ts, int n1, int n2){
        if((ts.autumn == n1) || (ts.winter == n1)){
            ts.player1 = true;
        }
        else ts.player1 = false;
        if((ts.autumn == n2) || (ts.winter == n2)){
            ts.player2 = true;
        }
        else ts.player2 = false;
    }

    public void updatePlayerConditions(TeamsState ts){
        if(ts.player1 == true) {
            //change position to team chosen
            if(ts.autumn == PlayerLobby.host){
                host.transform.position = AutumnTeamPosition.transform.position;
            }
            else host.transform.position = WinterTeamPosition.transform.position;
        }
        else {
            //change position to null
            host.transform.position = NullPosition.transform.position;
        }
        if(ts.player2 == true) {
            //change position to team chosen
            if(ts.autumn == PlayerLobby.guest){
                guest.transform.position = AutumnTeamPosition.transform.position;
            }
            else guest.transform.position = WinterTeamPosition.transform.position;
        }
        else {
            //change position to null
            guest.transform.position = NullPosition.transform.position;
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