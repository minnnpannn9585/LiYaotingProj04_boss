using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    public Image healthBar;          // 在 Inspector 里拖 Boss 血条填充图片

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (healthBar != null)
            healthBar.fillAmount = (float)currentHealth / maxHealth;

        Debug.Log($"Boss takes {damage} damage. Left: {currentHealth}");

        if (currentHealth <= 0)
        {
            // TODO: Boss 死亡处理
            Debug.Log("Boss Dead");
        }
    }
}