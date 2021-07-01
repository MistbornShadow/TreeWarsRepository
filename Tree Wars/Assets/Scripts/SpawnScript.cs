using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TW.SpawnBehavior
{
    public class SpawnScript : MonoBehaviour
    {
        public GameObject unitSpawn;

        public void SpawnKnight()
        {
            Instantiate(unitSpawn, transform.position, Quaternion.identity);
        }
    }
}

