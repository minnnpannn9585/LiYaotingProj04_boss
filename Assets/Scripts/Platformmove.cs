using UnityEngine;

public class MovingPlatformForward : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 10f;

    [Tooltip("�ѷ���� y �������㣬ֻ��ˮƽ����ǰ��")]
    public bool useHorizontalDirection = true;

    private Vector3 _dir = Vector3.forward;
    private bool _initialized;

    /// <summary>������ʱ���ⲿע���ƶ�������ٶȡ�</summary>
    public void Initialize(Vector3 worldDirection, float newSpeed)
    {
        _dir = worldDirection;
        if (useHorizontalDirection) _dir.y = 0f;
        if (_dir.sqrMagnitude < 1e-6f) _dir = Vector3.forward;
        _dir.Normalize();

        speed = newSpeed;
        _initialized = true;

        // ��ƽ̨���泯���˶����򣨽�Ӱ����ۣ�
        transform.rotation = Quaternion.LookRotation(_dir, Vector3.up);
    }

    void Update()
    {
        if (!_initialized) return;
        transform.position += _dir * speed * Time.deltaTime;
    }
}
