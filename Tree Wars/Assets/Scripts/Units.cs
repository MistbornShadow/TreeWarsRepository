using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TW.Units
{
    [CreateAssetMenu(fileName = "New Unit", menuName = "Create New Unit")]

    public class Units : ScriptableObject
    {
        public enum unitType
        {
            main,
            knight
        };

        public int color; //blue = 0, red = 1;

        public new string name;

        public unitType type;

        public GameObject unitSprite;

        public int health;
        public int attack;
    }
}

