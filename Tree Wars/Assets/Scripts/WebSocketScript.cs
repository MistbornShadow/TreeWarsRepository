using System.Collections;
using System.Collections.Generic;
using System;
using WebSocketSharp;
using UnityEngine;
using TW.Player;

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
        public Boolean full;
        public int gameID;
        public int player1;
        public int player2;
    }

    public class WebSocketScript : MonoBehaviour
    {
        public WebSocket ws;
        string gameID;

        PlayerScript player;

        int[] keys = new int[] {};

        void Start()
        {
            ws = new WebSocket("ws://localhost:8080");
            ws.Connect();
            Debug.Log("Connection made");
            ws.OnMessage += (sender, e) => {
                var data = JsonUtility.FromJson<DataObject>(e.Data);
                Debug.Log(e.Data);
                Debug.Log(data);
                switch(data.type){
                    case "playerID":
                        player.playerID = Int32.Parse(data.info);
                        break;
                    case "server_list":
                        string serverlist = data.info;
                        Debug.Log(serverlist);
                        break;
                    case "key":
                        int key = Int32.Parse(data.info);
                        Debug.Log(key);
                        addKey(key);
                        break;
                    case "create_server_list":
                        createServerList();
                        break;
                }
            };
        }

        void Update()
        {
            if (ws == null)
            {
                return;
            }
            else
            {
  
            }
        }

        public void addKey(int key){
            keys[keys.Length] = key;
        }

        public void SendSpawnMessage()
        {
            DataObject spawn = new DataObject();
            spawn.type = "spawn_unit";

            Debug.Log("Sending spawn message...");

            string serializeSpawn = JsonUtility.ToJson(spawn);
            ws.Send(serializeSpawn);
        }

        public void SendServerListRequest()
        {
            DataObject request = new DataObject();
            request.type = "get_server_list";

            string serializeRequest = JsonUtility.ToJson(request);
            ws.Send(serializeRequest);
        }

        public void SendCreateServerRequest()
        {
            DataObject request = new DataObject();
            request.type = "create_server";

            string serializeRequest = JsonUtility.ToJson(request);
            ws.Send(serializeRequest);
        }

        public void SendJoinRequest()
        {
            DataObject request = new DataObject();
            request.type = "join_server";

            string serializeRequest = JsonUtility.ToJson(request);
            ws.Send(serializeRequest);
        }

        public void createServerList(){
            foreach(int i in keys){
                Debug.Log(i);
            }
        }

        public void RecieveMessage()
        {

        }
    }
}