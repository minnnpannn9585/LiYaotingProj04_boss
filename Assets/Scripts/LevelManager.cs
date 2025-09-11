using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Skill01 skill01;
    public Skill02 skill02;
    public BossDashSkill skill03; // 第三技能（冲刺），由 LevelManager 控制 CastSkill()
    public float skillInterval = 2f;
    public float TimegapBetweenSkills = 1f; // 一技能与二技能之间的间隔（秒）

    float startTimer = 5f;
    bool started = false;


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
