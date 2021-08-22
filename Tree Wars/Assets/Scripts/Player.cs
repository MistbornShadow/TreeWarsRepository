using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TW.PlayerClass{
    public class Player
        {
            
            public int playerID;
            //if team is 1, then the player is Autumn. If 2, the player is winter
            public int team;
            
            public int resource;

            public Player(int thisPlayerId, int thisPlayerTeam){
                playerID = thisPlayerId;
                team = thisPlayerTeam;
                resource = 0;
            }

            public void updateResource(int newResource){
                this.resource = newResource;
            }
        }
}