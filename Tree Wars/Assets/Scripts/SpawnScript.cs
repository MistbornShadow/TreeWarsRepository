using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TW.SpawnBehavior
{
    public class SpawnScript : MonoBehaviour
    {
        public GameObject knight;
        
        public void mainSpawnFunction(string info){
            switch(info){
                case "knight":
                    SpawnKnight();
                    break;
                default:
                    Debug.Log("ERROR: Spawn function " + info);
                    break;
            }
        }
        public void SpawnKnight()
        {
            Instantiate(knight, transform.position, Quaternion.identity);
        }
    }
}

