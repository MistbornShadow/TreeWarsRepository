using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoaderScript : MonoBehaviour
{
    public void transitionToCreateJoin()
    {
         SceneManager.LoadScene("CreateJoin");
    }

    public void transitionToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void transitionToSearching()
    {
        SceneManager.LoadScene("Searching");
    }
}
