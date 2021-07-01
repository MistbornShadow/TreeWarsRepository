using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TW.Player;

public class lobbyMenuScript : MonoBehaviour
{
    public Text gameIDText;
    PlayerScript player;
    public void addGameID(){
        gameIDText.text = "Game ID: " + player.gameID;
    }
}
