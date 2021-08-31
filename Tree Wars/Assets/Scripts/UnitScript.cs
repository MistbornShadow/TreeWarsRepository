using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TW.NetworkBehavior;

namespace TW.Units
{   
    public class UnitScript : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public Units _unit;

        public int unitHealth;

        float timer = 0.0f;
        float delay = 1.0f;

        void Start(){
            unitHealth = getHealth();
        }

        public int getHealth(){
            return this._unit.health;
        }

        public int getAttack(){
            return this._unit.attack;
        }

        public int getColor(){
            return this._unit.color;
        }

        public string getName(){
            return this._unit.name;
        }

        public void loseHealth(int attack){
            Debug.Log("Unit losing " + attack + " health");
            this.unitHealth -= attack;
        }

        public bool death(){
            if(unitHealth <= 0){
                animator.SetBool("death", true);
                return true;
            }
            else return false;
        }

        void Update(){
            if(death()){
                timer += Time.deltaTime;
                if(timer >= delay)  {
                    if(WebSocketScript.checkIsHost()) WebSocketScript.sendUnitDeath(getName(), getColor());
                    GameObject.Destroy(this.gameObject);
                }
            }
        }

    }
}