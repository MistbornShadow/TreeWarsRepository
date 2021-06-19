using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TW.Units;

public class AutumnUnitBehaviors : MonoBehaviour
{
    public Units _unit;
    private GameObject Unit;
    private RaycastHit2D hit;
    [SerializeField] private Animator animator;

    public bool walking;
    public bool enemy;

    // Update is called once per frame
    void Update()
    {
        if (!characterFront())
        {
            walking = true;
            transform.Translate(Vector2.right * Time.deltaTime);
        }
        else
        {
            walking = false;
        }

        animator.SetBool("walking", walking);
        animator.SetBool("enemy", enemy);
    }

    private bool characterFront()
    {
        hit = Physics2D.Raycast(new Vector2(transform.position.x + 1, transform.position.y), Vector2.right, 1f);
        Debug.DrawRay(new Vector2(transform.position.x + 1, transform.position.y), Vector2.right, Color.green);

        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}