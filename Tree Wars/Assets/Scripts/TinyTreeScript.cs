using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyTreeScript : MonoBehaviour
{

    public int maxHealth = 20;
    public int currentHealth;

    public HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
