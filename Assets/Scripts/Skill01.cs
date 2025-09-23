using System.Collections;
using UnityEngine;

public class Skill01 : MonoBehaviour
{
    [Header("Attack settings")]
    [Tooltip("Angle of the fan attack in degrees")]
    public float fanAngle = 60f;
    [Tooltip("Every attack bullets count")]
    public int bulletsPerAttack = 5;
    [Tooltip("Bullet launch force range")]
    public float minLaunchForce = 10f;
    public float maxLaunchForce = 30f;

    [Header("Bullet settings")]
    [Tooltip("立方体子弹预制体")]
    public GameObject bulletPrefab;
    [Tooltip("Bullet launch point")]
    public Transform firePoint;

    [Header("Cast & Cooldown")]
    public float castDuration = 0.2f;     
    public float attackInterval = 3f;    

    public bool isCasting = false;
    private bool isOnCooldown = false;

    void Update()
    {
        // For testing
        if (Input.GetKeyDown(KeyCode.R))
        {
            TryCast();
        }
    }

    public void TryCast()
    {
        if (isCasting || isOnCooldown) return;
        StartCoroutine(CastSkill());
    }


    public IEnumerator CastSkill()
    {
        isCasting = true;
        isOnCooldown = true;

        PerformFanAttack();        

        yield return new WaitForSeconds(castDuration);
        isCasting = false;

        yield return new WaitForSeconds(attackInterval - castDuration); 
        isOnCooldown = false;
    }

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

    // Draw Gizmos
    void OnDrawGizmosSelected()
    {
        if (firePoint == null) return;

        Gizmos.color = Color.red;
        Vector3 direction = firePoint.forward;

        // scape showing
        Vector3 leftDirection = Quaternion.Euler(0, -fanAngle / 2, 0) * direction;
        Vector3 rightDirection = Quaternion.Euler(0, fanAngle / 2, 0) * direction;

        // scape showing
        Gizmos.DrawRay(firePoint.position, direction * 5f);
        Gizmos.DrawRay(firePoint.position, leftDirection * 5f);
        Gizmos.DrawRay(firePoint.position, rightDirection * 5f);
    }
}
