using UnityEngine;

public class BossExpression : MonoBehaviour
{
    [Header("表情对象")]
    public GameObject normalFace;
    public GameObject skill01Face;
    public GameObject skill02Face;
    public GameObject skill03Face; // 三技能表情

    [Header("表情控制")]
    [Tooltip("技能切换后表情保持的固定秒数（设0为立即切回）")]
    public float keepDuration;

    private Skill01 skill01Script;
    [SerializeField] private Skill02 skill02Script; // 二技能引用（可手动拖）
    [SerializeField] private BossDashSkill skill03Script; // 三技能引用（可手动拖，未拖则自动找）
    private float skillKeepUntil = -Mathf.Infinity;
    private int activeSkill = 0; // 0 = normal, 1 = skill01, 2 = skill02, 3 = skill03

    // 可选的 Awake fallback（非必须，只作容错）
    void Awake()
    {
        // 在子物体里找 Skill01
        skill01Script = GetComponentInChildren<Skill01>();
    if (skill02Script == null) skill02Script = FindObjectOfType<Skill02>();
    if (skill03Script == null) skill03Script = FindObjectOfType<BossDashSkill>();
    }

    void Update()
    {
        // 优先判断 skill01，再判断 skill02（优先级按需求可调整）
        if (skill01Script != null && skill01Script.isCasting)
        {
            // 技能触发时设置保持到的时间点
            skillKeepUntil = Time.time + keepDuration;
            activeSkill = 1;
            ShowSkill01();
        }
        else if (skill02Script != null && skill02Script.isCasting)
        {
            skillKeepUntil = Time.time;
            activeSkill = 2;
            ShowSkill02();
        }
        else if (skill03Script != null && skill03Script.isCasting)
        {
            skillKeepUntil = Time.time;
            activeSkill = 3;
            ShowSkill03();
        }
        else
        {
            // 未施放时若仍在保持期内继续显示最后的技能表情，否则恢复常态
            if (Time.time <= skillKeepUntil)
            {
                if (activeSkill == 1) ShowSkill01();
                else if (activeSkill == 2) ShowSkill02();
                else if (activeSkill == 3) ShowSkill03();
                else ShowNormal();
            }
            else
            {
                activeSkill = 0;
                ShowNormal();
            }
        }
    }

    private void ShowNormal()
    {
        if (normalFace && !normalFace.activeSelf) normalFace.SetActive(true);
        if (skill01Face && skill01Face.activeSelf) skill01Face.SetActive(false);
        if (skill02Face && skill02Face.activeSelf) skill02Face.SetActive(false);
    if (skill03Face && skill03Face.activeSelf) skill03Face.SetActive(false);
    }

    private void ShowSkill01()
    {
        if (normalFace && normalFace.activeSelf) normalFace.SetActive(false);
        if (skill01Face && !skill01Face.activeSelf) skill01Face.SetActive(true);
        if (skill02Face && skill02Face.activeSelf) skill02Face.SetActive(false);
    }

    private void ShowSkill02()
    {
        if (normalFace && normalFace.activeSelf) normalFace.SetActive(false);
        if (skill01Face && skill01Face.activeSelf) skill01Face.SetActive(false);
        if (skill02Face && !skill02Face.activeSelf) skill02Face.SetActive(true);
        if (skill03Face && skill03Face.activeSelf) skill03Face.SetActive(false);
    }

    private void ShowSkill03()
    {
        if (normalFace && normalFace.activeSelf) normalFace.SetActive(false);
        if (skill01Face && skill01Face.activeSelf) skill01Face.SetActive(false);
        if (skill02Face && skill02Face.activeSelf) skill02Face.SetActive(false);
        if (skill03Face && !skill03Face.activeSelf) skill03Face.SetActive(true);
    }
}