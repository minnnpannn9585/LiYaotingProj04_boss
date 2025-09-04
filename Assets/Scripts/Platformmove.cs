using UnityEngine;

public class MovingPlatformForward : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 10f;

    [Tooltip("把方向的 y 分量清零，只在水平面上前进")]
    public bool useHorizontalDirection = true;

    private Vector3 _dir = Vector3.forward;
    private bool _initialized;

    /// <summary>在生成时由外部注入移动方向和速度。</summary>
    public void Initialize(Vector3 worldDirection, float newSpeed)
    {
        _dir = worldDirection;
        if (useHorizontalDirection) _dir.y = 0f;
        if (_dir.sqrMagnitude < 1e-6f) _dir = Vector3.forward;
        _dir.Normalize();

        speed = newSpeed;
        _initialized = true;

        // 让平台“面朝”运动方向（仅影响外观）
        transform.rotation = Quaternion.LookRotation(_dir, Vector3.up);
    }

    void Update()
    {
        if (!_initialized) return;
        transform.position += _dir * speed * Time.deltaTime;
    }
}
