using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;
using UnityEngine;

namespace TW.NetworkBehavior
{
    [System.Serializable]
    public class DataObject
    {
        public string type;
        public string player;
    }


    public class WebSocketScript : MonoBehaviour
    {
        public WebSocket ws;
        void Start()
        {
            ws = new WebSocket("ws://localhost:8080");
            ws.OnMessage += (sender, e) => {
                Debug.Log("Message recieved from " + ((WebSocket)sender).Url + ", Data: " + e.Data);
            };
            ws.Connect();
        }

        void Update()
        {
            if (ws == null)
            {
                return;
            }
            if (Input.GetButton("Create Game"))
            {

            }
        }

        public void SendKnightSpawnMessage(string player)
        {
            DataObject knight = new DataObject();
            knight.type = "knight";
            knight.player = player;
            string serializeKnight = JsonUtility.ToJson(knight);
            Debug.Log("Sending KnightSpawn message...");
            ws.Send(serializeKnight);
        }

        public void RecieveMessage()
        {
            
        }
    }
}