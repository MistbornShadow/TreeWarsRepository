using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TW.PlayerClass;
using TW.NetworkBehavior;
using UnityEngine.UI;
public class GameBehavior : MonoBehaviour
{
    Player player;

    public Text resourceText;

    float timer;
    bool isHost;

    string updateCommand;
    string updateObjectString;
    void Start()
    {
        timer = 0.0f;
        player = new Player(WebSocketScript.playerID, WebSocketScript.findTeamSelected());
        isHost = WebSocketScript.checkIsHost();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 1.0 && isHost) WebSocketScript.checkForUpdate();
        if(WebSocketScript.isUpdateNeeded){
            updateCommand = WebSocketScript.getUpdateCommand();
            updateObjectString = WebSocketScript.getUpdateObjectString();
            switch (updateCommand){
                case "resources_update":
                    var resourceChange = JsonUtility.FromJson<int>(updateObjectString);
                    player.updateResource(resourceChange);
                    setResourceText();
                    break;
                default:
                    Debug.Log("Error: update commmand: " + updateCommand);
                    break;
            }
        }
    }

    public void setResourceText(){
        resourceText.text = player.resource.ToString();
    }
}
