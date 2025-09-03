using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamSweeper : MonoBehaviour
{
    [Header("Path")]
    public Transform sweepStart;     // 扫描起点
    public Transform sweepEnd;       // 扫描终点

    [Header("Timing")]
    public float sweepDuration = 2f;     // 单次从起到终的时间
    public float pauseAtEnds = 0.3f;     // 两端停顿
    public bool pingPong = true;         // 往返(来回)或每次都从起点重置

    private float t = 0f;
    private bool forward = true;
    private float pauseTimer = 0f;

    private void Start()
    {
        if (sweepStart != null) transform.position = sweepStart.position;
    }

    private void Update()
    {
        if (sweepStart == null || sweepEnd == null || sweepDuration <= 0f) return;

        // 端点停顿
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            return;
        }

        // 进度推进
        t += (Time.deltaTime / sweepDuration) * (forward ? 1f : -1f);
        t = Mathf.Clamp01(t);

        // 位置插值
        transform.position = Vector3.Lerp(sweepStart.position, sweepEnd.position, t);

        // 到端点处理
        if (Mathf.Approximately(t, 1f) || Mathf.Approximately(t, 0f))
        {
            pauseTimer = pauseAtEnds;

            if (pingPong)
            {
                forward = !forward; // 来回扫
            }
            else
            {
                // 只从起点扫到终点：每次结束后重置
                forward = true;
                t = 0f;
                transform.position = sweepStart.position;
            }
        }
    }

    // 可视化路径
    private void OnDrawGizmosSelected()
    {
        if (sweepStart != null && sweepEnd != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(sweepStart.position, sweepEnd.position);
            Gizmos.DrawWireCube(sweepStart.position, new Vector3(0.2f, 0.2f, 0.2f));
            Gizmos.DrawWireCube(sweepEnd.position, new Vector3(0.2f, 0.2f, 0.2f));
        }
    }
}
