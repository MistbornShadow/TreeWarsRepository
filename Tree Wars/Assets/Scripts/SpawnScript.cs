using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TW.SpawnBehavior
{
    public class SpawnScript : MonoBehaviour
    {
        public GameObject knight;

        public void SpawnKnight()
        {
            Instantiate(knight, transform.position, Quaternion.identity);
        }
    }
}

