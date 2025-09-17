using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Skill01 skill01;
    public Skill02 skill02;
    public BossDashSkill skill03;

    public float skillInterval = 2f;          // 伤害阶段结束到下一轮技能开始的间隔
    // public float TimegapBetweenSkills = 1f; // （可删除）旧通用间隔
    public float gapAfterSkill01 = 1f;        // Skill01(两次) 到 Skill02 的间隔
    public float gapAfterSkill02 = 1f;        // Skill02 到 Skill03 的间隔
    public float damagePhaseDuration = 5f;    // 伤害窗口持续时间

    public float startTimer;
    bool started = false;

    public GameObject playerDamagePhase;
    public GameObject bossParent;

    bool inDamagePhase = false;
    public bool IsInDamagePhase => inDamagePhase; // 供外部读取

    void Start()
    {
        if (playerDamagePhase == null)
        {
            var t = transform.Find("PlayerDamagePhase");
            if (t != null) playerDamagePhase = t.gameObject;
        }

        if (bossParent == null)
        {
            var bp = GameObject.Find("BossParent");
            if (bp != null) bossParent = bp;
        }

        if (playerDamagePhase != null)
            playerDamagePhase.SetActive(false);
        else
            Debug.LogWarning("[LevelManager] playerDamagePhase 未找到");
    }

    void Update()
    {
        // 只用于第一轮的启动计时
        if (!started)
        {
            if (startTimer > 0f)
                startTimer -= Time.deltaTime;

            if (startTimer <= 0f)
            {
                StartCoroutine(BossRoutine());
                started = true;
            }
        }
    }

    IEnumerator BossRoutine()
    {
        while (true)
        {
            if (skill01 != null)
            {
                for (int i = 0; i < 2; i++)
                    yield return StartCoroutine(skill01.CastSkill());
            }

            yield return new WaitForSeconds(gapAfterSkill01);   // 改：独立间隔

            if (skill02 != null)
                yield return StartCoroutine(skill02.CastSkill());

            yield return new WaitForSeconds(gapAfterSkill02);   // 改：独立间隔

            if (skill03 != null)
                yield return StartCoroutine(skill03.CastSkill());

            yield return StartCoroutine(StartDamagePhaseRoutine());
            yield return new WaitForSeconds(skillInterval);
        }
    }

    IEnumerator StartDamagePhaseRoutine()
    {
        if (inDamagePhase) yield break;
        inDamagePhase = true;
        ShowPlayerDamagePhase();
        yield return new WaitForSeconds(damagePhaseDuration);
        EndDamagePhase();
    }

    void ShowPlayerDamagePhase()
    {
        if (playerDamagePhase == null) return;

        playerDamagePhase.SetActive(true);
        if (bossParent != null)
            bossParent.SetActive(false);
    }

    void EndDamagePhase()
    {
        if (!inDamagePhase) return;
        inDamagePhase = false;

        if (playerDamagePhase != null)
            playerDamagePhase.SetActive(false);

        if (bossParent != null)
            bossParent.SetActive(true);
    }

    public void ForceEndDamagePhase()
    {
        EndDamagePhase();
    }
}
