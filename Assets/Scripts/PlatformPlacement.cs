using UnityEngine;

/// <summary>
/// �ڳ���ڵ����ҷ�Χ�����һ�����ɵ㣨�߶� = ���������߶ȣ���
/// �ѽű����ڡ�����ڣ�SpawnPoint������ʹ�á�
/// </summary>
public class RandomXSpawner : MonoBehaviour
{
    [Header("Spawn Range (local X)")]
    [Tooltip("���Ұ���ף���ƽ̨���� [-xRange, +xRange] ���������")]
    public float xRange = 3f;

    [Tooltip("��Ե��������������߽����ɣ�0 ��ʾ��������")]
    public float edgePadding = 0f;

    [Tooltip("���벽�������� 0.5�����뵽 0.5m �ı�����0 ��ʾ�����룩")]
    public float snapStep = 0f;

    [Header("Direction Options")]
    [Tooltip("��ʹ��ˮƽ�淽�򣨺���y�������鿪���Ի���ȶ���������")]
    public bool useHorizontalForward = true;

    [Header("Gizmos")]
    public bool drawGizmos = true;
    public Color gizmoColor = new Color(0.2f, 1f, 0.4f, 1f);
    public float gizmoEdgeSphere = 0.15f;
    public float gizmoTick = 0.2f;

    /// <summary>
    /// ��ȡһ��������ɵ㣨�������꣩��
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
        // �߶�ֱ���� SpawnPoint ������ Y�������޸�
        return pos;
    }

    /// <summary>
    /// ��ȡ���ڶ��塰���ҡ���ǰ������
    /// �Ե�ǰ����� forward Ϊ��׼����ѡ��ѹƽ��ˮƽ�档
    /// </summary>
    private Vector3 GetForward()
    {
        Vector3 fwd = transform.forward;
        if (useHorizontalForward) fwd.y = 0f;
        if (fwd.sqrMagnitude < 1e-6f) fwd = Vector3.forward;
        return fwd.normalized;
    }

    // ���༭�����ӻ�
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

        // ���߶�
        Gizmos.color = gizmoColor;
        Gizmos.DrawLine(a, b);

        // �߽�С��
        Gizmos.DrawSphere(a, gizmoEdgeSphere);
        Gizmos.DrawSphere(b, gizmoEdgeSphere);

        // ���Ŀ̶�
        Vector3 up = Vector3.up * gizmoTick;
        Gizmos.DrawLine(center - up, center + up);
    }
}