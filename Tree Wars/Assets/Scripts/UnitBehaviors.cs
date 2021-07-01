using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TW.Units;

public class UnitBehaviors : MonoBehaviour
{
    public Units _unit;
    private GameObject Unit;
    private RaycastHit2D hit;

    public UnitScript front;
    [SerializeField] private Animator animator;

    public bool walking;
    public bool enemy;

    float timer = 0.0f;
    float delay = 2.0f;

    // Update is called once per frame
    void Update()
    {
        if (!characterFront())
        {
            walking = true;
            if (this._unit.color == 0)
            {
                transform.Translate(Vector2.right * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector2.left * Time.deltaTime);
            }
        }
        else
        {
            walking = false;
            if(enemyFront()){
                enemy = true;
                attackEnemy();
            }
            else{
                enemy = false;
            }
        }

        animator.SetBool("enemy", enemy);
        animator.SetBool("walking", walking);
    }

    private bool characterFront()
    {

        if (this._unit.color == 0)
        {
            hit = Physics2D.Raycast(new Vector2(transform.position.x + 1, transform.position.y), Vector2.right, 1f);
            Debug.DrawRay(new Vector2(transform.position.x + 1, transform.position.y), Vector2.right, Color.green);
        }
        else
        {
            hit = Physics2D.Raycast(new Vector2(transform.position.x - 1, transform.position.y), Vector2.left, 1f);
            Debug.DrawRay(new Vector2(transform.position.x - 1, transform.position.y), Vector2.left, Color.green);
        }


        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool enemyFront(){
        front = hit.collider.gameObject.GetComponent(typeof(UnitScript)) as UnitScript;
        
        if(front.getColor() != this._unit.color){
            return true;
        }
        else return false;
    }

    public void attackEnemy(){
        timer += Time.deltaTime;
        Debug.Log(timer);
        if(timer > delay){
            front.loseHealth(this._unit.attack);
            timer = 0.0f;
        }
    }
}