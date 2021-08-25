using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TW.PlayerClass;
using TW.NetworkBehavior;
using UnityEngine.UI;
using TW.SpawnBehavior;
public class GameBehavior : MonoBehaviour
{
    Player player;
    public GameObject winterSpawn;
    public GameObject autumnSpawn;

    public Text resourceText;

    float timerUpdate;
    float timerResourceIncrease;
    bool isHost;

    string updateCommand;
    string updateObjectString;

    public class SpawnUnit{
        public int team;
        public string type;
        public int newResource;
    }
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
                case "spawn_unit_winter":
                    var winterSpawnObj = JsonUtility.FromJson<SpawnUnit>(updateObjectString);
                    spawnUnit(winterSpawnObj.type, winterSpawn);
                    if(player.team == winterSpawnObj.team) player.updateResource(winterSpawnObj.newResource);
                    break;
                case "spawn_unit_autumn":
                    var autumnSpawnObj = JsonUtility.FromJson<SpawnUnit>(updateObjectString);
                    spawnUnit(autumnSpawnObj.type, winterSpawn);
                    if(player.team == autumnSpawnObj.team) player.updateResource(autumnSpawnObj.newResource);
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

    public void sendSpawnUnitRequest(string unit){
        if(checkResourceCost(unit)) WebSocketScript.sendSpawnUnitRequest(unit);
        else return;
    }

    public void spawnUnit(string unit, GameObject spawner){
        spawner.GetComponent<SpawnScript>().mainSpawnFunction(unit);
    }

    private bool checkResourceCost(string purchase){
        switch (purchase){
            case "knight":
                if(player.resource >= 30) return true;
                break;
            default:
                Debug.Log("ERROR: purchase check " + purchase);
                break;
        }
        return false;
    }
}
