using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TW.PlayerClass;
using TW.NetworkBehavior;
using UnityEngine.UI;
public class GameBehavior : MonoBehaviour
{
    Player player;

    public Text resourceText;

    float timerUpdate;
    float timerResourceIncrease;
    bool isHost;

    string updateCommand;
    string updateObjectString;
    void Start()
    {
        timerUpdate = 0.0f;
        timerResourceIncrease = 0.0f;
        player = new Player(WebSocketScript.playerID, WebSocketScript.findTeamSelected());
        isHost = WebSocketScript.checkIsHost();
        resourceText.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        timerUpdate += Time.deltaTime;
        timerResourceIncrease += Time.deltaTime;
        if(timerUpdate > 1.0 && isHost) {
            WebSocketScript.checkForUpdate();
            timerUpdate = 0.0f;
        }
        if(timerResourceIncrease > 3.0 && isHost){
            WebSocketScript.sendResourceIncreaseRequest();
            timerResourceIncrease = 0.0f;
        }
        if(WebSocketScript.isUpdateNeeded){
            updateCommand = WebSocketScript.getUpdateCommand();
            updateObjectString = WebSocketScript.getUpdateObjectString();
            switch (updateCommand){
                case "resources_update":
                    var resourceChange = Int32.Parse(updateObjectString);
                    player.updateResource(resourceChange);
                    setResourceText();
                    break;
                default:
                    Debug.Log("Error: update commmand: " + updateCommand);
                    break;
            }
            WebSocketScript.isUpdateNeeded = false;
        }
    }

    public void setResourceText(){
        resourceText.text = player.resource.ToString();
    }
}
