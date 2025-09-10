using System.Collections;
using UnityEngine;

public class Skill01 : MonoBehaviour
{
    [Header("攻击设置")]
    [Tooltip("扇形攻击的角度范围（度）")]
    public float fanAngle = 60f;
    [Tooltip("每次攻击发射的子弹数量")]
    public int bulletsPerAttack = 5;
    [Tooltip("攻击间隔时间（秒）")]
    public float attackInterval = 3f;
    [Tooltip("子弹发射力度")]
    public float minLaunchForce = 10f;
    public float maxLaunchForce = 30f;

    [Header("子弹设置")]
    [Tooltip("立方体子弹预制体")]
    public GameObject bulletPrefab;
    [Tooltip("子弹发射点")]
    public Transform firePoint;

    [Header("施放设置")]
    [Tooltip("本次技能表现持续时间（秒）。LevelManager 连续两次 StartCoroutine(CastSkill()) 时，两次间隔等于此值")]
    public float castDuration = 0.2f; // 新增，可在 Inspector 调整

    [HideInInspector] public bool isCasting = false;   // BossExpression 可以检测

    void Update()
    {
        // 手动测试用：按 R 启动技能
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(CastSkill());
        }
    }

    /// <summary>
    /// 提供给 LevelManager 调用的接口
    /// </summary>
    public IEnumerator CastSkill()
    {
        isCasting = true;

        PerformFanAttack();

        // 技能表现持续时间，改为使用 castDuration（可在 Inspector 调整）
        yield return new WaitForSeconds(castDuration);

        isCasting = false;
    }

    /// <summary>
    /// 执行扇形攻击
    /// </summary>
    private void PerformFanAttack()
    {
        if (firePoint == null || bulletPrefab == null) return;

        Vector3 forwardDir = firePoint.TransformDirection(Vector3.forward);
        Vector3 rightDir = firePoint.TransformDirection(Vector3.right);

        for (int i = 0; i < bulletsPerAttack; i++)
        {
            float angleRadians = Random.Range(-fanAngle / 2f, fanAngle / 2f) * Mathf.Deg2Rad;
            Vector3 bulletDirection = (forwardDir * Mathf.Cos(angleRadians) + rightDir * Mathf.Sin(angleRadians)).normalized;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(bulletDirection));
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                float force = Random.Range(minLaunchForce, maxLaunchForce);
                bulletRb.AddForce(bulletDirection * force, ForceMode.Impulse);
            }
        }
    }

    // 编辑器中绘制扇形攻击范围的Gizmos
    void OnDrawGizmosSelected()
    {
        if (firePoint == null) return;

        Gizmos.color = Color.red;
        Vector3 direction = firePoint.forward;

        // 计算扇形两边的方向
        Vector3 leftDirection = Quaternion.Euler(0, -fanAngle / 2, 0) * direction;
        Vector3 rightDirection = Quaternion.Euler(0, fanAngle / 2, 0) * direction;

        // 绘制扇形范围示意
        Gizmos.DrawRay(firePoint.position, direction * 5f);
        Gizmos.DrawRay(firePoint.position, leftDirection * 5f);
        Gizmos.DrawRay(firePoint.position, rightDirection * 5f);
    }
}
