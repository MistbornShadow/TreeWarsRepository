using System.Collections;
using System.Collections.Generic;
using System;
using WebSocketSharp;
using UnityEngine;
using TW.ServerObject;
using UnityEngine.SceneManagement;

namespace TW.NetworkBehavior
{

    [Serializable]
    public class Wrapper<T>
    {
        public T[] Items;
    }

    [System.Serializable]
    public class DataObject
    {
        public string type;
        public string info;
    }

    [System.Serializable]
    public class TeamsState
    {
        public int autumn;
        public int winter;

        public bool player1;
        public bool player2;
    }

    public class Server{
        public bool full;
        public int gameID;
        public int player1;
        public int player2;
        public int aSelect;
        public int wSelect;
    }

    public static class WebSocketScript
    {
        public static WebSocket ws;

        public static bool loadedServers;

        public static GameObject ServerObj;

        public static string serverlist = null;

        public static List<int> serverList = new List<int>();

        public static int playerID;
        public static int gameID = 0;
        public static int guestID = -1;
        public static int hostID = -1;
        public static bool joined = false;
        public static int title = -1;
        public static bool isKick = false;
        public static bool isServerCreated = false;

        public static bool startGame = false;

        //class to keep track of whether the team has been selected
        public static TeamsState ts = new TeamsState();

        public static List<int> keys = new List<int>();

        public static void generateWebsocketHost(){
            ws = new WebSocket("ws://Hitokiri-Batosai:80");
            ws.Connect();
            Debug.Log("Connection made");
            generatePlayerID();
            loadedServers = false;
        }

        public static void generateWebsocketGuest(){
            ws = new WebSocket("ws://Hitokiri-Batosai:80");
            Debug.Log("Attempting connection...");
            ws.Connect();
            Debug.Log("Connection made");
            generatePlayerID();
            loadedServers = false;        
        }


        public static void recieveMessage(string s)
        {
            var data = JsonUtility.FromJson<DataObject>(s);
            switch(data.type){
                case "send_player_ID":
                    SendHostPlayerID();
                    break;
                case "generate_player_ID":
                    generatePlayerID();
                    break;
                case "host_player_ID":
                    hostPlayerID(Int32.Parse(data.info));
                    break;
                case "generate_game_ID":
                    generateGameID();
                    break;
                case "server":
                    Debug.Log(data.info);
                    //int temp = JsonUtility.FromJson<int>(data.info);
                    serverList.Add(Int32.Parse(data.info));
                    break;
                case "key":
                    int key = Int32.Parse(data.info);
                    addKey(key);
                    break;
                case "create_server_list":
                    createServerList();
                    break;
                case "guest_connect":
                    guestID = Int32.Parse(data.info);
                    break;
                case "game_ID":
                    gameID = Int32.Parse(data.info);
                    break;
                case "successful_join":
                    Debug.Log("successful join recieved");
                    if(title != 1){
                        gameID = Int32.Parse(data.info);
                        joined = true;
                    }
                    else{
                        joined = true;
                    }
                    ts.autumn = -1;
                    ts.winter = -1;
                    break;
                case "unsuccessful_join":
                    joined = false;
                    break;
                case "update_teams":
                    recieveTeamsState(data.info);
                    break;
                case "guest_player_ID":
                    guestID = Int32.Parse(data.info);
                    break;
                case "start_game":
                    startGame = true;
                    break;
                case "kick":
                    if(title == 1){
                        guestID = -1;
                    }
                    else{
                        hostID = -1;
                    }
                    joined = false;
                    ts.autumn = -1;
                    ts.winter = -1;
                    isKick = true;
                    break;
                case "server_created":
                    isServerCreated = true;
                    break;
                case "message":
                    Debug.Log(data.info);
                    break;
                default:
                    Debug.Log("UKNOWN REQUEST: " + data.type);
                    break;
            }
        }

        public static void gameExit() {
            DataObject obj = new DataObject();
            obj.type = "game_exit";
            obj.info = playerID.ToString() + " " + gameID.ToString();
            string serializeObj = JsonUtility.ToJson(obj);
            ws.Send(serializeObj);
        }

        public static void addKey(int key){
            Debug.Log(key);
            keys.Add(key);
        }

        public static void hostPlayerID(int id){
            hostID = id;
        }

        public static void startGameFunction(){
            DataObject obj = new DataObject();
            obj.type = "start_game";
            obj.info = gameID.ToString();

            Debug.Log("Starting Game");

            string serializeObj = JsonUtility.ToJson(obj);
            ws.Send(serializeObj);
        }

        public static void generatePlayerID(){
            playerID = UnityEngine.Random.Range(0, 100000);
            DataObject num = new DataObject();
            num.type = "player_ID";
            num.info = playerID.ToString();

            Debug.Log("Sending player Id.");

            string serializeID = JsonUtility.ToJson(num);
            ws.Send(serializeID);
        }

        public static void generateGameID(){
            gameID = UnityEngine.Random.Range(0, 100000);
            DataObject num = new DataObject();
            num.type = "game_ID";
            num.info = gameID.ToString();

            Debug.Log("Sending game Id.");

            string serializeID = JsonUtility.ToJson(num);
            ws.Send(serializeID);
        }

        public static void SendSpawnMessage()
        {
            DataObject spawn = new DataObject();
            spawn.type = "spawn_unit";

            Debug.Log("Sending spawn message...");

            string serializeSpawn = JsonUtility.ToJson(spawn);
            ws.Send(serializeSpawn);
        }

        public static void SendServerListRequest()
        {
            DataObject request = new DataObject();
            request.type = "get_server_list";

            string serializeRequest = JsonUtility.ToJson(request);
            ws.Send(serializeRequest);
        }

        public static void SendCreateServerRequest()
        {
            DataObject request = new DataObject();
            request.type = "create_server";
            request.info = playerID.ToString();
            title = 1;

            string serializeRequest = JsonUtility.ToJson(request);
            ws.Send(serializeRequest);
        }

        public static void SendJoinRequest(int gID)
        {
            DataObject request = new DataObject();
            request.type = "join_server";
            gameID = gID;
            request.info = gID.ToString() + " " + playerID.ToString();
            Debug.Log(request.info);

            string serializeRequest = JsonUtility.ToJson(request);
            ws.Send(serializeRequest);
        }

        public static void SendHostPlayerID(){
            DataObject request = new DataObject();
            request.type = "host_player_ID";
            request.info = playerID.ToString() + " " + gameID.ToString();

            string serializeRequest = JsonUtility.ToJson(request);
            ws.Send(serializeRequest);
        }

        public static void requestHostID(int gameID){
            DataObject request = new DataObject();
            request.type = "request_host";
            request.info = gameID.ToString();
            guestID = playerID;

            string serializeRequest = JsonUtility.ToJson(request);
            ws.Send(serializeRequest);
        }

        public static void requestGuestID(int gameID){
            DataObject request = new DataObject();
            request.type = "request_guest";
            request.info = gameID.ToString();

            string serializeRequest = JsonUtility.ToJson(request);
            ws.Send(serializeRequest);
        }

        public static void updateTeamCondition(int i){
            DataObject request = new DataObject();
            request.type = "update_teams";
            request.info = gameID.ToString() + " " + i.ToString() + " " + playerID.ToString();

            string serializeRequest = JsonUtility.ToJson(request);
            ws.Send(serializeRequest);            
        }

        public static void recieveTeamsState(string s){
            var newTS = JsonUtility.FromJson<TeamsState>(s);
            ts = newTS;
        }

        public static void createServerList(){
            loadedServers = true;
        }
    }
}