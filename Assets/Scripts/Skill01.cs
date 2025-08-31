using System.Collections;
using System.Collections.Generic;
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
    public float launchForce = 20f;
    [Tooltip("子弹存在时间（秒）")]
    public float bulletLifetime = 5f;

    [Header("子弹设置")]
    [Tooltip("立方体子弹预制体")]
    public GameObject bulletPrefab;
    [Tooltip("子弹发射点")]
    public Transform firePoint;



    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)){
            PerformFanAttack();
        }
    }

    /// <summary>
    /// 执行扇形攻击
    /// </summary>
    void PerformFanAttack()
    {
        Vector3 forwardDir = firePoint.TransformDirection(Vector3.forward);
        // 获取Boss的右侧方向（X轴）
        Vector3 rightDir = firePoint.TransformDirection(Vector3.right);

        for (int i = 0; i < bulletsPerAttack; i++)
        {
            // 计算随机角度（转换为弧度）
            float angleRadians = Random.Range(-fanAngle / 2, fanAngle / 2) * Mathf.Deg2Rad;
            
            // 关键修复：使用三角函数计算围绕Y轴的方向
            // 这确保子弹在Y轴周围形成完美的扇形分布
            Vector3 bulletDirection = 
                forwardDir * Mathf.Cos(angleRadians) + 
                rightDir * Mathf.Sin(angleRadians);
            bulletDirection.Normalize();

            print(bulletDirection);
            // 实例化子弹
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, 
                Quaternion.LookRotation(bulletDirection));
            
            // 应用力
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.AddForce(bulletDirection * launchForce, ForceMode.Impulse);

            // 设置自动销毁
            Destroy(bullet, bulletLifetime);
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
