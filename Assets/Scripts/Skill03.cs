using System.Collections;
using UnityEngine;

public class BossDashSkill : MonoBehaviour
{
    [Header("冲刺子物体")]
    [SerializeField] private GameObject dashChildObjectlr; // 左右方向冲刺模板
    [SerializeField] private GameObject dashChildObjectbf; // 前后方向冲刺模板
    GameObject dashChildObject;   // 主（第一个）冲刺子物体（场景中已有）
    GameObject dashChildObject2;  // 运行时克隆出的第二个冲刺子物体

    [Header("场景区域参数 (固定值)")]
    [SerializeField] private float areaWidth = 37f;    // 场景宽度
    [SerializeField] private float areaDepth = 22f;    // 场景深度
    [SerializeField] private Vector3 areaCenter;      // 场景中心点

    [Header("冲刺参数")]
    [SerializeField] private float dashSpeed = 20f;   // 冲刺速度
    [SerializeField] private float dashInterval = 2f; // 冲刺间隔时间
    [SerializeField] private int totalDashes = 5;     // 总冲刺次数
    [Tooltip("是否让第二条轨道独立随机（而不是简单平行偏移）")]
    [SerializeField] private bool secondTrackIndependent = true;
    [Tooltip("当使用独立随机时，两条轨道中点之间的最小距离 (不足则重新随机，最多尝试5次)")]
    [SerializeField] private float minTrackCenterDistance = 4f;

    [Header("预警参数")]
    [SerializeField] private GameObject warningPrefab; // 预警效果预制体
    [SerializeField] private float warningDuration = 1f; // 预警显示时间
    [SerializeField] private Vector3 warningScale = new Vector3(1, 0.1f, 2); // 预警效果缩放
    [Tooltip("预警显示的 Y 高度（世界坐标），默认 -2.5 与原实现一致，可在 Inspector 调整）")]
    [SerializeField] private float warningHeight = -2.5f;

    [Header("冲刺高度设置")]
    [Tooltip("冲刺子物体生成时的 Y 高度（世界坐标）。在 Inspector 中调整此值以改变生成高度。")]
    [SerializeField] private float dashSpawnHeight;

    [Header("控制")]
    [Tooltip("是否自动释放技能（Inspector 可配置）。将其设为 false 可临时禁止自动释放，仍可通过 ActivateSkill 或 LevelManager 的 CastSkill 手动触发。")]
    [SerializeField] private bool autoRelease = false;

    // 标记技能是否正在运行（避免重复触发）
    private bool isRunning = false;
    // 提供给表情控制读取（true 表示技能整体处于释放阶段）
    public bool isCasting { get; private set; } = false;

    private enum DashDirection { LeftRight, ForwardBack }
    private enum SkillState { Ready, Warning, Dashing, Cooldown }

    private SkillState currentState;
    private DashDirection currentDirection;
    private DashDirection currentDirection2; // 第二条轨道方向（独立模式下可能不同）
    private int dashCount;
    private Vector3 dashStartPos;   // 第一条轨道起点
    private Vector3 dashEndPos;     // 第一条轨道终点
    private Vector3 dashStartPos2;  // 第二条轨道起点
    private Vector3 dashEndPos2;    // 第二条轨道终点
    private GameObject currentWarning;   // 第一条轨道预警
    private GameObject currentWarning2;  // 第二条轨道预警

    private void Awake()
    {
        currentState = SkillState.Ready;
        dashCount = 0;
    }

    private void Update()
    {
        // 手动测试：按 Q 触发整套多段冲刺
        if (Input.GetKeyDown(KeyCode.Q) && !isRunning)
        {
            StartCoroutine(CastSkill());
        }
        // 仅在 Inspector 开启自动释放时由 Update 启动协程式释放
        if (autoRelease && !isRunning)
        {
            StartCoroutine(CastSkill());
        }
    }

    // 用于 LevelManager 的协程接口，执行一次完整的技能阶段（包含多次冲刺）
    public IEnumerator CastSkill()
    {
        if (isRunning) yield break;
        isRunning = true;
    isCasting = true;

        dashCount = 0;
        currentState = SkillState.Ready;

        for (int i = 0; i < totalDashes; i++)
        {
            // 准备并显示预警
            StartNextDash();

            // 等待预警时间
            yield return new WaitForSeconds(warningDuration);

            // 进入冲刺（销毁预警）
            if (currentWarning != null)
            {
                Destroy(currentWarning);
                currentWarning = null;
            }
            if (currentWarning2 != null)
            {
                Destroy(currentWarning2);
                currentWarning2 = null;
            }

            // 开始移动子物体到终点
            if (dashChildObject != null)
            {
                currentState = SkillState.Dashing;
                // 同步移动：直到两个都到达终点
                bool allArrived = false;
                while (!allArrived)
                {
                    if (dashChildObject != null)
                        dashChildObject.transform.position = Vector3.MoveTowards(
                            dashChildObject.transform.position,
                            dashEndPos,
                            dashSpeed * Time.deltaTime
                        );
                    if (dashChildObject2 != null)
                        dashChildObject2.transform.position = Vector3.MoveTowards(
                            dashChildObject2.transform.position,
                            dashEndPos2,
                            dashSpeed * Time.deltaTime
                        );

                    float d1 = dashChildObject != null ? Vector3.Distance(dashChildObject.transform.position, dashEndPos) : 0f;
                    float d2 = dashChildObject2 != null ? Vector3.Distance(dashChildObject2.transform.position, dashEndPos2) : 0f;
                    allArrived = d1 <= 0.5f && d2 <= 0.5f;
                    yield return null;
                }

                // 结束本次冲刺
                dashChildObject.SetActive(false);
                if (dashChildObject2 != null)
                {
                    Destroy(dashChildObject2);
                    dashChildObject2 = null;
                }
            }

            dashCount++;
            currentState = SkillState.Cooldown;

            // 冷却间隔
            yield return new WaitForSeconds(dashInterval);
            currentState = SkillState.Ready;
        }

        // 技能阶段结束，短延时后重置状态（保持与之前 ResetSkill 行为兼容）
        yield return new WaitForSeconds(0.01f);
        ResetSkill();
        isRunning = false;
    isCasting = false;
    }

    // 开始下一次冲刺
    private void StartNextDash()
    {
    // 根据冲刺方向选择物体朝向（左右或前后）
        currentDirection = (DashDirection)Random.Range(0, 2);

        if (currentDirection == DashDirection.LeftRight)
        {
            dashChildObject = dashChildObjectlr;
        }
        else if (currentDirection == DashDirection.ForwardBack)
        {
            dashChildObject = dashChildObjectbf;
        }

        // 第一条轨道
        dashStartPos = GetEdgeStartPosition(currentDirection);
        dashEndPos = CalculateDashEndPosition(currentDirection, dashStartPos);

        if (secondTrackIndependent)
        {
            // 独立随机第二条轨道（方向可同可不同）
            int attempts = 0;
            bool ok = false;
            do
            {
                currentDirection2 = (DashDirection)Random.Range(0, 2);
                dashStartPos2 = GetEdgeStartPosition(currentDirection2);
                dashEndPos2 = CalculateDashEndPosition(currentDirection2, dashStartPos2);
                Vector3 c1 = (dashStartPos + dashEndPos) * 0.5f;
                Vector3 c2 = (dashStartPos2 + dashEndPos2) * 0.5f;
                if (Vector3.Distance(c1, c2) >= minTrackCenterDistance)
                {
                    ok = true;
                    break;
                }
                attempts++;
            } while (attempts < 5);
            if (!ok)
            {
                // 失败则退回到平行偏移逻辑
                currentDirection2 = currentDirection;
                dashStartPos2 = dashStartPos;
                dashEndPos2 = dashEndPos;
                ApplyParallelFallback();
            }
        }
        else
        {
            // 旧的平行偏移逻辑（保持原行为）
            currentDirection2 = currentDirection;
            dashStartPos2 = dashStartPos;
            dashEndPos2 = dashEndPos;
            ApplyParallelFallback();
        }

        // 准备冲刺子物体（第一个 + 第二个独立方向的选择）
        if (dashChildObject != null)
        {
            dashChildObject.transform.position = dashStartPos;
            // 选择第二条轨道的模板：如果方向不同则根据方向拿对应模板，否则仍然克隆第一个
            GameObject secondTemplate = dashChildObject;
            if (secondTrackIndependent && currentDirection2 != currentDirection)
            {
                secondTemplate = GetTemplateForDirection(currentDirection2);
            }

            if (dashChildObject2 != null)
            {
                Destroy(dashChildObject2);
                dashChildObject2 = null;
            }
            if (secondTemplate != null)
            {
                dashChildObject2 = Instantiate(secondTemplate, transform);
                dashChildObject2.transform.position = dashStartPos2;
            }

            // 统一朝向（如果需要）仅根据各自轨道方向计算
            Vector3 flatDir1 = dashEndPos - dashStartPos; flatDir1.y = 0f;
            Vector3 flatDir2 = dashEndPos2 - dashStartPos2; flatDir2.y = 0f;
            if (flatDir1.sqrMagnitude > 0.0001f)
            {
                //dashChildObject.transform.rotation = Quaternion.LookRotation(flatDir1);
            }
            if (dashChildObject2 != null && flatDir2.sqrMagnitude > 0.0001f)
            {
                //dashChildObject2.transform.rotation = Quaternion.LookRotation(flatDir2);
            }

            dashChildObject.SetActive(true);
            if (dashChildObject2 != null) dashChildObject2.SetActive(true);
        }

    // 显示预警（两条轨道）
    ShowWarning();

        currentState = SkillState.Warning;
    }

    // 新增：带方向参数的起点计算，用于独立第二轨逻辑
    private Vector3 GetEdgeStartPosition(DashDirection dir)
    {
        // 复用原无参逻辑：暂复制本类旧实现并按传入方向选择
        Vector3 startPos = areaCenter;
        if (dir == DashDirection.LeftRight)
        {
            startPos.x = areaCenter.x + areaWidth / 2; // 右侧边缘（原注释有左右混用，这里保持行为）
            startPos.z = areaCenter.z + Random.Range(-areaDepth / 2, areaDepth / 2);
        }
        else
        {
            startPos.z = areaCenter.z - areaDepth / 2; // 上侧边缘
            startPos.x = areaCenter.x + Random.Range(-areaWidth / 2, areaWidth / 2);
        }
        startPos.y = dashSpawnHeight;
        return startPos;
    }

    // 原无参版本仍被旧代码使用（若无其它引用可考虑删除）

    private Vector3 CalculateDashEndPosition(DashDirection dir, Vector3 start)
    {
        Vector3 endPos = start;
        if (dir == DashDirection.LeftRight)
        {
            endPos.x = areaCenter.x - areaWidth / 2; // 另一侧边缘
        }
        else
        {
            endPos.z = areaCenter.z + areaDepth / 2; // 下侧边缘
        }
        return endPos;
    }

    // 根据方向返回对应模板（可能为 null）
    private GameObject GetTemplateForDirection(DashDirection dir)
    {
        return dir == DashDirection.LeftRight ? dashChildObjectlr : dashChildObjectbf;
    }


    // 平行偏移回退函数（或未开启独立模式时）
    private void ApplyParallelFallback()
    {
        if (currentDirection2 == DashDirection.LeftRight)
        {
            float minZ = areaCenter.z - areaDepth / 2f;
            float maxZ = areaCenter.z + areaDepth / 2f;
            float candidate = dashStartPos.z;
            if (candidate > maxZ || candidate < minZ)
                candidate = dashStartPos.z;
            candidate = Mathf.Clamp(candidate, minZ + 0.01f, maxZ - 0.01f);
            dashStartPos2.z = candidate;
            dashEndPos2.z = candidate;
        }
        else
        {
            float minX = areaCenter.x - areaWidth / 2f;
            float maxX = areaCenter.x + areaWidth / 2f;
            float candidate = dashStartPos.x;
            if (candidate > maxX || candidate < minX)
                candidate = dashStartPos.x;
            candidate = Mathf.Clamp(candidate, minX + 0.01f, maxX - 0.01f);
            dashStartPos2.x = candidate;
            dashEndPos2.x = candidate;
        }
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

        // 第一条
        currentWarning = Instantiate(warningPrefab);
        if (currentWarning != null)
        {
            Vector3 warningPos = Vector3.Lerp(dashStartPos, dashEndPos, 0.5f);
            warningPos.y = dashStartPos.y + warningHeight;
            currentWarning.transform.position = warningPos;
            Vector3 flatDir = dashEndPos - dashStartPos; flatDir.y = 0f;
            if (flatDir.sqrMagnitude > 0.0001f)
                currentWarning.transform.rotation = Quaternion.LookRotation(flatDir);
            float dashLength = Vector3.Distance(dashStartPos, dashEndPos);
            Vector3 ls = currentWarning.transform.localScale; ls.z = Mathf.Max(0.01f, dashLength); currentWarning.transform.localScale = ls;
            currentWarning.transform.SetParent(transform, true);
        }

        // 第二条
        currentWarning2 = Instantiate(warningPrefab);
        if (currentWarning2 != null)
        {
            Vector3 warningPos2 = Vector3.Lerp(dashStartPos2, dashEndPos2, 0.5f);
            warningPos2.y = dashStartPos2.y + warningHeight;
            currentWarning2.transform.position = warningPos2;
            Vector3 flatDir2 = dashEndPos2 - dashStartPos2; flatDir2.y = 0f;
            if (flatDir2.sqrMagnitude > 0.0001f)
                currentWarning2.transform.rotation = Quaternion.LookRotation(flatDir2);
            float dashLength2 = Vector3.Distance(dashStartPos2, dashEndPos2);
            Vector3 ls2 = currentWarning2.transform.localScale; ls2.z = Mathf.Max(0.01f, dashLength2); currentWarning2.transform.localScale = ls2;
            currentWarning2.transform.SetParent(transform, true);
        }
    }

    // 开始冲刺
    private void StartDashing()
    {
        if (currentWarning != null)
        {
            Destroy(currentWarning);
            currentWarning = null;
        }
        if (currentWarning2 != null)
        {
            Destroy(currentWarning2);
            currentWarning2 = null;
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
        if (dashChildObject2 != null)
        {
            Destroy(dashChildObject2);
            dashChildObject2 = null;
        }

        dashCount++;
        currentState = SkillState.Cooldown;

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
    // 不在这里清 isCasting，让外部在 CastSkill 协程结束时清理
    }

    // 外部调用：开始释放技能
    public void ActivateSkill()
    {
        if (currentState == SkillState.Ready)
        {
            dashCount = 0;
            StartNextDash();
            // 如果是通过 ActivateSkill 触发单次流程，也标记施放（外部可在别处重置）
            if (!isCasting) isCasting = true;
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
