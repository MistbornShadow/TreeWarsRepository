using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TW.PlayerLobby;

public class PlayerLobbyController : MonoBehaviour
{
    public void addHostID(int num){
        PlayerLobby.host = num;
    }
    public void addGuestID(int num){
        PlayerLobby.guest = num;
    }
    public bool checkFull(){
        if((PlayerLobby.host != -1) && (PlayerLobby.guest != -1)) PlayerLobby.full = true;
    }
}
