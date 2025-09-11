using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth =100;



    public void TakeDamage(int damage)
    {
        Debug.Log("Player takes " + damage + " damage.");
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            //Die();
        }
    }
}
