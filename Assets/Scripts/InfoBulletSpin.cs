using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletSpin : MonoBehaviour
{
    public Vector3 randomAxisMin = new Vector3(-1, -1, -1);
    public Vector3 randomAxisMax = new Vector3( 1,  1,  1);
    public float minAngularSpeed = 10f;   // 角速度（度/秒等价转成弧度后内部处理）
    public float maxAngularSpeed = 360f;

    void Start()
    {
        var rb = GetComponent<Rigidbody>();
        // 随机轴
        Vector3 axis = new Vector3(
            Random.Range(randomAxisMin.x, randomAxisMax.x),
            Random.Range(randomAxisMin.y, randomAxisMax.y),
            Random.Range(randomAxisMin.z, randomAxisMax.z)
        ).normalized;
        float speedDeg = Random.Range(minAngularSpeed, maxAngularSpeed);
        // 直接设角速度（弧度/秒）
        rb.angularVelocity = axis * speedDeg * Mathf.Deg2Rad;
        // 或者用 AddTorque（若想受质量影响）:
        // rb.AddTorque(axis * speedDeg, ForceMode.VelocityChange);
    }
}
