using System.Collections;
using System.Collections.Generic;
using System;
using WebSocketSharp;
using UnityEngine;
using TW.Player;
using TW.ServerObject;

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

    public class Server{
        public bool full;
        public int gameID;
        public int player1;
        public int player2;
    }

    public static class WebSocketScript
    {
        public static WebSocket ws;

        public static bool loadedServers;

        public static GameObject ServerObj;

        public static string serverlist = null;

        public static List<Server> serverList = new List<Server>();
        public static PlayerScript player;

        public static int playerID;
        public static int gameID = 0;
        public static int guestID;
        public static bool joined;

        public static List<int> keys = new List<int>();

        public static void generateWebsocket(){
            ws = new WebSocket("ws://localhost:8080");
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
                case "generate_game_ID":
                    generateGameID();
                    break;
                case "server":
                    Debug.Log(data.info);
                    Server temp = JsonUtility.FromJson<Server>(data.info);
                    serverList.Add(temp);
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
                    joined = true;
                    break;
                case "unsuccessful_join":
                    joined = false;
                    break;
                default:
                    Debug.Log("UKNOWN REQUEST: " + data.type);
                    break;
            }
        }

        public static void gameExit() {
            DataObject obj = new DataObject();
            obj.type = "game_exit";
            obj.info = playerID.ToString();
            string serializeObj = JsonUtility.ToJson(obj);
            ws.Send(serializeObj);
        }

        public static void addKey(int key){
            Debug.Log(key);
            keys.Add(key);
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

            string serializeRequest = JsonUtility.ToJson(request);
            ws.Send(serializeRequest);
        }

        public static void SendJoinRequest(int gameID)
        {
            DataObject request = new DataObject();
            request.type = "join_server";
            request.info = gameID.ToString() + " " + playerID.ToString();
            Debug.Log(request.info);

            string serializeRequest = JsonUtility.ToJson(request);
            ws.Send(serializeRequest);
        }

        public static void SendHostPlayerID(){
            DataObject request = new DataObject();
            request.type = "host_player_ID";
            request.info = playerID.ToString();

            string serializeRequest = JsonUtility.ToJson(request);
            ws.Send(serializeRequest);
        }

        public static void createServerList(){
            loadedServers = true;
        }
    }
}