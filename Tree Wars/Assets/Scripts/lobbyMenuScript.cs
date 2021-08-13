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
            checkPlayerConditions(WebSocketScript.ts, lobby.host, lobby.guest);
            updatePlayerConditions(WebSocketScript.ts);
        }
    }

    public void checkPlayerConditions(TeamsState ts, int n1, int n2){
        if((ts.autumn || ts.winter) == n1){
            ts.player1 = true;
        }
        else ts.player1 = false;
        if((ts.autumn || ts.winter) == n2){
            ts.player2 = true;
        }
        else ts.player2 = false;
    }

    public void updatePlayerConditions(TeamsState teamsState){
        if(ts.player1 = true) {
            //change position to team chosen
            if(ts.autumn == lobby.host){
                host.transform = AutumnTeamPosition.transform;
            }
            else host.transform = WinterTeamPosition.transform;
        }
        else {
            //change position to null
            host.transform = NullPosition.transform;
        }
        if(ts.player2 = true) {
            //change position to team chosen
            if(ts.autumn == lobby.guest){
                guest.transform = AutumnTeamPosition.transform;
            }
            else guest.transform = WinterTeamPosition.transform;
        }
        else {
            //change position to null
            guest.transform = NullPosition.transform;
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