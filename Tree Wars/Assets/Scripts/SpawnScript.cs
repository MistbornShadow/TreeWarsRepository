using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TW.SpawnBehavior
{
    public class SpawnScript : MonoBehaviour
    {
        public GameObject unitKnight;
        // Update is called once per frame

        public void SpawnKnight()
        {
            Instantiate(unitKnight, transform.position, Quaternion.identity);
        }
    }
}

