using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TW.Player
{
    [CreateAssetMenu(fileName = "Player", menuName = "Create New Player")]
    public class PlayerScript : ScriptableObject
    {
        public int playerID;

        public int playerNum;

        public int gameID;
    }

}
