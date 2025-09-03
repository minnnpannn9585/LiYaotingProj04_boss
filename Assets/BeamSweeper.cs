using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamSweeper : MonoBehaviour
{
    [Header("Path")]
    public Transform sweepStart;     // ɨ�����
    public Transform sweepEnd;       // ɨ���յ�

    [Header("Timing")]
    public float sweepDuration = 2f;     // ���δ����յ�ʱ��
    public float pauseAtEnds = 0.3f;     // ����ͣ��
    public bool pingPong = true;         // ����(����)��ÿ�ζ����������

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

        // �˵�ͣ��
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            return;
        }

        // �����ƽ�
        t += (Time.deltaTime / sweepDuration) * (forward ? 1f : -1f);
        t = Mathf.Clamp01(t);

        // λ�ò�ֵ
        transform.position = Vector3.Lerp(sweepStart.position, sweepEnd.position, t);

        // ���˵㴦��
        if (Mathf.Approximately(t, 1f) || Mathf.Approximately(t, 0f))
        {
            pauseTimer = pauseAtEnds;

            if (pingPong)
            {
                forward = !forward; // ����ɨ
            }
            else
            {
                // ֻ�����ɨ���յ㣺ÿ�ν���������
                forward = true;
                t = 0f;
                transform.position = sweepStart.position;
            }
        }
    }

    // ���ӻ�·��
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
