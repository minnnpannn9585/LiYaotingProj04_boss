using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    public Image healthBar;
    float lerpSpeed;

    public void TakeDamage(int damage)
    {
        Debug.Log("Player takes " + damage + " damage.");

        currentHealth -= damage;

        healthBar.fillAmount = (float)currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            //Die();
        }

        lerpSpeed = 3f * Time.deltaTime;


    }

}
