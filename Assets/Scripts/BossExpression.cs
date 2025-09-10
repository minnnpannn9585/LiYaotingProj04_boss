using UnityEngine;

public class BossExpression : MonoBehaviour
{
    [Header("表情对象")]
    public GameObject normalFace;
    public GameObject skill01Face;

    [Header("表情控制")]
    [Tooltip("技能切换后表情保持的固定秒数（设0为立即切回）")]
    public float keepDuration;

    private Skill01 skill01Script;
    private float skillKeepUntil = -Mathf.Infinity;

    void Awake()
    {
        // 在子物体里找 Skill01
        skill01Script = GetComponentInChildren<Skill01>();
    }
    
    void Update()
    {
        if (skill01Script != null && skill01Script.isCasting)
        {
            // 技能触发时设置保持到的时间点
            skillKeepUntil = Time.time + keepDuration;
            ShowSkill01();
        }
        else
        {
            // 未施放时若仍在保持期内继续显示技能表情，否则恢复常态
            if (Time.time <= skillKeepUntil)
                ShowSkill01();
            else
                ShowNormal();
        }
    }

    private void ShowNormal()
    {
        if (normalFace && !normalFace.activeSelf) normalFace.SetActive(true);
        if (skill01Face && skill01Face.activeSelf) skill01Face.SetActive(false);
    }

    private void ShowSkill01()
    {
        if (normalFace && normalFace.activeSelf) normalFace.SetActive(false);
        if (skill01Face && !skill01Face.activeSelf) skill01Face.SetActive(true);
    }
}