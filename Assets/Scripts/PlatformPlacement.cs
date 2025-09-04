using UnityEngine;

/// <summary>
/// 在出射口的左右范围内随机一个生成点（高度 = 出射口自身高度）。
/// 把脚本挂在“出射口（SpawnPoint）”上使用。
/// </summary>
public class RandomXSpawner : MonoBehaviour
{
    [Header("Spawn Range (local X)")]
    [Tooltip("左右半宽（米），平台会在 [-xRange, +xRange] 内随机生成")]
    public float xRange = 3f;

    [Tooltip("边缘内缩，避免紧贴边界生成（0 表示不内缩）")]
    public float edgePadding = 0f;

    [Tooltip("对齐步长（例如 0.5：对齐到 0.5m 的倍数；0 表示不对齐）")]
    public float snapStep = 0f;

    [Header("Direction Options")]
    [Tooltip("仅使用水平面方向（忽略y），建议开启以获得稳定的左右轴")]
    public bool useHorizontalForward = true;

    [Header("Gizmos")]
    public bool drawGizmos = true;
    public Color gizmoColor = new Color(0.2f, 1f, 0.4f, 1f);
    public float gizmoEdgeSphere = 0.15f;
    public float gizmoTick = 0.2f;

    /// <summary>
    /// 获取一个随机生成点（世界坐标）。
    /// </summary>
    public Vector3 GetSpawnPosition()
    {
        Vector3 fwd = GetForward();
        Vector3 right = Vector3.Cross(Vector3.up, fwd).normalized;

        float half = Mathf.Max(0f, xRange - Mathf.Max(0f, edgePadding));
        float offset = Random.Range(-half, +half);

        if (snapStep > 0.0001f)
        {
            offset = Mathf.Round(offset / snapStep) * snapStep;
        }

        Vector3 pos = transform.position + right * offset;
        // 高度直接用 SpawnPoint 的自身 Y，不做修改
        return pos;
    }

    /// <summary>
    /// 获取用于定义“左右”的前进方向。
    /// 以当前物体的 forward 为基准，可选择压平到水平面。
    /// </summary>
    private Vector3 GetForward()
    {
        Vector3 fwd = transform.forward;
        if (useHorizontalForward) fwd.y = 0f;
        if (fwd.sqrMagnitude < 1e-6f) fwd = Vector3.forward;
        return fwd.normalized;
    }

    // 仅编辑器可视化
    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Vector3 fwd = GetForward();
        Vector3 right = Vector3.Cross(Vector3.up, fwd).normalized;

        float half = Mathf.Max(0f, xRange);
        float pad = Mathf.Clamp(edgePadding, 0f, half);

        Vector3 center = transform.position;
        Vector3 a = center + right * (-half + pad);
        Vector3 b = center + right * (+half - pad);

        // 主线段
        Gizmos.color = gizmoColor;
        Gizmos.DrawLine(a, b);

        // 边界小球
        Gizmos.DrawSphere(a, gizmoEdgeSphere);
        Gizmos.DrawSphere(b, gizmoEdgeSphere);

        // 中心刻度
        Vector3 up = Vector3.up * gizmoTick;
        Gizmos.DrawLine(center - up, center + up);
    }
}