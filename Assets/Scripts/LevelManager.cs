using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Image enemyHealthImage;
    public float enemyHealth = 100f;
    public Skill01 skill01;
    public Skill02 skill02;
    public BossDashSkill skill03; // 第三技能（冲刺），由 LevelManager 控制 CastSkill()
    public float skillInterval = 2f;
    public float TimegapBetweenSkills = 1f; // 一技能与二技能之间的间隔（秒）

    public float startTimer;
    bool started = false;

    public void EnemyTakeDamage()
    {
        enemyHealth -= 10f;
        enemyHealthImage.fillAmount = enemyHealth / 100f;
        if (enemyHealth <= 0f)
        {
            // 敌人死亡逻辑
            Debug.Log("Enemy Defeated!");
            // 可以添加更多的死亡处理代码，例如播放动画、掉落物品等
        }
    }

    void Update()
    {
        startTimer -= Time.deltaTime;
        
        if(startTimer <= 0f && !started)
        {

            StartCoroutine(BossRoutine());
            started = true;
        }
    }



    IEnumerator BossRoutine()
    {
        while (true)
        {
            if (skill01 != null)
            {
                // 每次一技能阶段连放两次（等待每次 CastSkill 完成）
                yield return StartCoroutine(skill01.CastSkill());
                yield return StartCoroutine(skill01.CastSkill());
            }

            // 在一技能与二技能之间等待
            yield return new WaitForSeconds(TimegapBetweenSkills);

            if (skill02 != null)
            {
                // 二技能阶段
                yield return StartCoroutine(skill02.CastSkill());
            }

            yield return new WaitForSeconds(TimegapBetweenSkills);

            // 第三技能（冲刺），若存在则调用
            if (skill03 != null)
            {
                // 给一点间隔，保证表现上位于二技能之后
                yield return StartCoroutine(skill03.CastSkill());
            }

            yield return new WaitForSeconds(skillInterval);
        }
    }
}
