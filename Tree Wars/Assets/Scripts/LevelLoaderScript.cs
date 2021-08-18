using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TW.NetworkBehavior;

public class LevelLoaderScript : MonoBehaviour
{
    public void transitionToCreateJoin()
    {
         SceneManager.LoadScene("CreateJoin");
    }

    public void transitionToLobbyAsHost()
    {
        WebSocketScript.generateWebsocketHost();
        WebSocketScript.generateGameID();
        WebSocketScript.title = 1;
        WebSocketScript.ws.OnMessage += (sender, e) => {
            string s = e.Data;
            WebSocketScript.recieveMessage(s);
        };
        SceneManager.LoadScene("Lobby");
    }

    public void transitionToLobbyAsGuest(){
        WebSocketScript.ws.OnMessage += (sender, e) => {
            string s = e.Data;
            WebSocketScript.recieveMessage(s);
        };
        WebSocketScript.title = 2;
        SceneManager.LoadScene("Lobby");
    }

    public void transitionToSearching()
    {
        SceneManager.LoadScene("Searching");
    }
}
