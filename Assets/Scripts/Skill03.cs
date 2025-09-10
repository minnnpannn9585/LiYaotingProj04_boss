using UnityEngine;

public class BossDashSkill : MonoBehaviour
{
    [Header("冲刺子物体")]
    [SerializeField] private GameObject dashChildObject; // 负责冲刺的子物体

    [Header("场景区域参数 (固定值)")]
    [SerializeField] private float areaWidth = 37f;    // 场景宽度
    [SerializeField] private float areaDepth = 22f;    // 场景深度
    [SerializeField] private Vector3 areaCenter;      // 场景中心点

    [Header("冲刺参数")]
    [SerializeField] private float dashSpeed = 20f;   // 冲刺速度
    [SerializeField] private float dashInterval = 2f; // 冲刺间隔时间
    [SerializeField] private int totalDashes = 5;     // 总冲刺次数

    [Header("预警参数")]
    [SerializeField] private GameObject warningPrefab; // 预警效果预制体
    [SerializeField] private float warningDuration = 1f; // 预警显示时间
    [SerializeField] private Vector3 warningScale = new Vector3(1, 0.1f, 2); // 预警效果缩放
    [Tooltip("预警显示的 Y 高度（世界坐标），默认 -2.5 与原实现一致，可在 Inspector 调整）")]
    [SerializeField] private float warningHeight = -2.5f;

    [Header("冲刺高度设置")]
    [Tooltip("冲刺子物体生成时的 Y 高度（世界坐标）。在 Inspector 中调整此值以改变生成高度。")]
    [SerializeField] private float dashSpawnHeight;

    private enum DashDirection { LeftRight, ForwardBack }
    private enum SkillState { Ready, Warning, Dashing, Cooldown }

    private SkillState currentState;
    private DashDirection currentDirection;
    private int dashCount;
    private float stateTimer;
    private Vector3 dashStartPos;
    private Vector3 dashEndPos;
    private GameObject currentWarning;

    private void Awake()
    {
        currentState = SkillState.Ready;
        dashCount = 0;

        // 初始化冲刺子物体状态
        if (dashChildObject != null)
        {
            dashChildObject.SetActive(false);
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case SkillState.Ready:
                if (dashCount < totalDashes)
                {
                    StartNextDash();
                }
                break;

            case SkillState.Warning:
                stateTimer += Time.deltaTime;
                if (stateTimer >= warningDuration)
                {
                    StartDashing();
                }
                break;

            case SkillState.Dashing:
                // 移动冲刺子物体
                if (dashChildObject != null)
                {
                    float distance = Vector3.Distance(dashChildObject.transform.position, dashEndPos);
                    if (distance > 0.5f)
                    {
                        dashChildObject.transform.position = Vector3.MoveTowards(
                            dashChildObject.transform.position,
                            dashEndPos,
                            dashSpeed * Time.deltaTime
                        );
                    }
                    else
                    {
                        EndDashing();
                    }
                }
                else
                {
                    // 如果没有设置冲刺子物体，结束冲刺
                    EndDashing();
                }
                break;

            case SkillState.Cooldown:
                stateTimer += Time.deltaTime;
                if (stateTimer >= dashInterval)
                {
                    currentState = SkillState.Ready;
                }
                break;
        }
    }

    // 开始下一次冲刺
    private void StartNextDash()
    {
        // 随机选择冲刺方向（左右或前后）
        currentDirection = (DashDirection)Random.Range(0, 2);

        // 确定冲刺起点（边缘随机位置）和终点
        dashStartPos = GetEdgeStartPosition();
        dashEndPos = CalculateDashEndPosition();

        // 准备冲刺子物体
        if (dashChildObject != null)
        {
            dashChildObject.transform.position = dashStartPos;
            // 水平朝向终点，避免俯仰
            Vector3 flatDir = dashEndPos - dashStartPos;
            flatDir.y = 0f;
            if (flatDir.sqrMagnitude > 0.0001f)
                dashChildObject.transform.rotation = Quaternion.LookRotation(flatDir);
            dashChildObject.SetActive(true); // 激活子物体
        }

        // 显示预警
        ShowWarning();

        currentState = SkillState.Warning;
        stateTimer = 0f;
    }

    // 获取边缘起点（基于固定场景区域）
    private Vector3 GetEdgeStartPosition()
    {
        Vector3 startPos = areaCenter;
        
        if (currentDirection == DashDirection.LeftRight)
        {
            // 左右方向：左侧边缘，Z轴（深度）随机
            startPos.x = areaCenter.x + areaWidth / 2; // 左侧边缘X坐标
            // 在深度范围内随机（-areaDepth/2 到 areaDepth/2）
            startPos.z = areaCenter.z + Random.Range(-areaDepth / 2, areaDepth / 2);
        }
        else
        {
            // 前后方向：上侧边缘，X轴（宽度）随机
            startPos.z = areaCenter.z - areaDepth / 2; // 上侧边缘Z坐标
            // 在宽度范围内随机（-areaWidth/2 到 areaWidth/2）
            startPos.x = areaCenter.x + Random.Range(-areaWidth / 2, areaWidth / 2);
        }
        
        // 使用可配置的生成高度（世界坐标）
        startPos.y = dashSpawnHeight;
        
        return startPos;
    }

    // 计算冲刺终点（基于固定场景区域）
    private Vector3 CalculateDashEndPosition()
    {
        Vector3 endPos = dashStartPos;

        if (currentDirection == DashDirection.LeftRight)
        {
            // 左右方向：从左侧边缘冲刺到右侧边缘
            endPos.x = areaCenter.x - areaWidth / 2;
        }
        else
        {
            // 前后方向：从上侧边缘冲刺到下侧边缘
            endPos.z = areaCenter.z + areaDepth / 2;
        }

        return endPos;
    }

    // 显示路径预警（仅调整长度 Z，不改变预制体的 X/Y 缩放，但保证朝向正确）
    private void ShowWarning()
    {
        if (warningPrefab == null) return;

        currentWarning = Instantiate(warningPrefab);
        if (currentWarning == null) return;

        // 位置：中点，高度基于起点高度 + 偏移
        Vector3 warningPos = Vector3.Lerp(dashStartPos, dashEndPos, 0.5f);
        warningPos.y = dashStartPos.y + warningHeight;
        currentWarning.transform.position = warningPos;

        // 水平朝向终点，避免俯仰（只改变旋转，不动 X/Y 缩放）
        Vector3 flatDir = dashEndPos - dashStartPos;
        flatDir.y = 0f;
        if (flatDir.sqrMagnitude > 0.0001f)
            currentWarning.transform.rotation = Quaternion.LookRotation(flatDir);

        // 仅调整本地 Z 缩放（长度）
        float dashLength = Vector3.Distance(dashStartPos, dashEndPos);
        Vector3 ls = currentWarning.transform.localScale;
        ls.z = Mathf.Max(0.01f, dashLength);
        currentWarning.transform.localScale = ls;

        // 保持世界变换不变地附加到本对象
        currentWarning.transform.SetParent(transform, true);
    }

    // 开始冲刺
    private void StartDashing()
    {
        if (currentWarning != null)
        {
            Destroy(currentWarning);
            currentWarning = null;
        }

        currentState = SkillState.Dashing;
    }

    // 结束冲刺
    private void EndDashing()
    {
        // 关闭冲刺子物体
        if (dashChildObject != null)
        {
            dashChildObject.SetActive(false);
        }

        dashCount++;
        currentState = SkillState.Cooldown;
        stateTimer = 0f;

        if (dashCount >= totalDashes)
        {
            enabled = false;
            Invoke(nameof(ResetSkill), 5f);
        }
    }

    // 重置技能
    private void ResetSkill()
    {
        dashCount = 0;
        currentState = SkillState.Ready;
        enabled = true;
    }

    // 外部调用：开始释放技能
    public void ActivateSkill()
    {
        if (currentState == SkillState.Ready)
        {
            dashCount = 0;
            StartNextDash();
        }
    }

    // 绘制场景区域和冲刺路径的Gizmos
    private void OnDrawGizmosSelected()
    {
        // 绘制场景区域边界
        Gizmos.color = Color.green;
        Vector3 areaMin = new Vector3(
            areaCenter.x - areaWidth / 2,
            areaCenter.y,
            areaCenter.z - areaDepth / 2
        );
        Vector3 areaMax = new Vector3(
            areaCenter.x + areaWidth / 2,
            areaCenter.y,
            areaCenter.z + areaDepth / 2
        );

        // 绘制区域边框
        Gizmos.DrawLine(new Vector3(areaMin.x, areaMin.y, areaMin.z), new Vector3(areaMax.x, areaMin.y, areaMin.z));
        Gizmos.DrawLine(new Vector3(areaMax.x, areaMin.y, areaMin.z), new Vector3(areaMax.x, areaMin.y, areaMax.z));
        Gizmos.DrawLine(new Vector3(areaMax.x, areaMin.y, areaMax.z), new Vector3(areaMin.x, areaMin.y, areaMax.z));
        Gizmos.DrawLine(new Vector3(areaMin.x, areaMin.y, areaMax.z), new Vector3(areaMin.x, areaMin.y, areaMin.z));

        // 绘制当前冲刺路径
        if (currentState != SkillState.Ready)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(dashStartPos, dashEndPos);
        }
    }



}
