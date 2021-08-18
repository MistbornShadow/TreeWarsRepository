using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TW.NetworkBehavior;
using WebSocketSharp;
using TW.PlayerLobby;
using UnityEngine.SceneManagement;

public class lobbyMenuScript : MonoBehaviour
{
    public Text gameIDText;
    public PlayerLobbyController lobby;

    public GameObject host;
    public GameObject guest;

    public bool startGame;

    private float time;

    //positions for the player objects upon clicking on the team indicated
    public GameObject AutumnTeamPosition;
    public GameObject WinterTeamPosition;
    public GameObject Null1Position;
    public GameObject Null2Position;

    public int gameID;
    void Start(){
        addGameID();
        addTitle();
        WebSocketScript.ws.OnMessage += (sender, e) => {
            string s = e.Data;
            WebSocketScript.recieveMessage(s);
        };
    }

    void Awake(){
        time = 0.0f;
    }

    void Update(){

        if(WebSocketScript.hostID != -1 && PlayerLobby.host == -1){
            lobby.addHostID(WebSocketScript.hostID);
        }
        else if(WebSocketScript.hostID == -1){
            WebSocketScript.requestHostID(WebSocketScript.gameID);
        }
        

        if(WebSocketScript.guestID != -1 && PlayerLobby.guest == -1){
            lobby.addGuestID(WebSocketScript.guestID);
        }
        if(WebSocketScript.joined){
            updatePlayerConditions(WebSocketScript.ts);
        }
        
        if(WebSocketScript.startGame){
            SceneManager.LoadScene("GameScene");
        }
    }

    void OnApplicationQuit() {
        WebSocketScript.gameExit();
    }

    public void updatePlayerConditions(TeamsState ts){
        if(ts.autumn == -1 && ts.winter == -1){
            host.transform.position = Null1Position.transform.position;
            guest.transform.position = Null2Position.transform.position;
            startGame = false;
        }
        else if(ts.autumn != -1 && ts.winter == -1) {
            //change position to team chosen
            if(ts.autumn == PlayerLobby.host){
                host.transform.position = AutumnTeamPosition.transform.position;
                guest.transform.position = Null2Position.transform.position;
                startGame = false;
            }
            else {
                guest.transform.position = AutumnTeamPosition.transform.position;
                host.transform.position = Null1Position.transform.position;
                startGame = false;
            }
        }
        else if(ts.winter != -1 && ts.autumn == -1) {
            //change position to team chosen
            if(ts.winter == PlayerLobby.host){
                host.transform.position = WinterTeamPosition.transform.position;
                guest.transform.position = Null2Position.transform.position;
                startGame = false;
            }
            else {
                guest.transform.position = WinterTeamPosition.transform.position;
                host.transform.position = Null1Position.transform.position;
                startGame = false;
            }
        }
        else{
            if(ts.autumn == PlayerLobby.host){
                host.transform.position = AutumnTeamPosition.transform.position;
                guest.transform.position = WinterTeamPosition.transform.position;
                startGame = true;
            }
            else {
                guest.transform.position = AutumnTeamPosition.transform.position;
                host.transform.position = WinterTeamPosition.transform.position;
                startGame = true;
            }
        }        
    }

    public void startGameFunction(){
        if(startGame && WebSocketScript.playerID == WebSocketScript.hostID){
            WebSocketScript.startGameFunction();
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