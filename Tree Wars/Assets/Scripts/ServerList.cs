using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TW.NetworkBehavior;

namespace TW.ServerObject{
    public class ServerList : MonoBehaviour
    {
        [SerializeField]
        public GameObject server;
        public GameObject parent;

        void Start(){
            while(true){
                if(WebSocketScript.loadedServers){
                    CreateList();
                    break;
                }
            }
        }
        public void CreateList(){
            Debug.Log("Creating ServerList");
            foreach(int ser in WebSocketScript.serverList){

                GameObject obj = Instantiate(server) as GameObject;

                obj.SetActive(true);
                obj.GetComponent<ServerScript>().SetID("Game ID: " + ser);
                obj.GetComponent<ServerScript>().setInt(ser);
                obj.transform.SetParent(parent.transform, false);
            }
        }
    }
}