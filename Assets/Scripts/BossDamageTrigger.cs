using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamageTrigger : MonoBehaviour
{
    public int damage = 10;                  // 可调
    public string damageTag = "Damage";      // 玩家攻击碰撞体使用的 Tag
    BossHealth bossHealth;

    void Awake()
    {
        bossHealth = FindObjectOfType<BossHealth>();
        if (bossHealth == null)
            Debug.LogError("BossDamageTrigger: 没找到 BossHealth");
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(damageTag)) return;
        if (bossHealth == null) return;

        bossHealth.TakeDamage(damage);
    }
}
