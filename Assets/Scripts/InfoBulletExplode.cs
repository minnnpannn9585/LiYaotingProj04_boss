using System.Collections;
using UnityEngine;

public class ExplosionVisualOnly : MonoBehaviour
{
    public float BulletLiveTime;
    public GameObject particlePrefab;

    // 新增：粒子保留时间与音效
    public float particleDuration = 3f;
    public AudioClip explosionSound;
    public float explosionVolume = 1f;

    void Start()
    {
        StartCoroutine(DoVisualAfterDelay());
    }

    IEnumerator DoVisualAfterDelay()
    {
        yield return new WaitForSeconds(BulletLiveTime);
        if (particlePrefab != null)
        {
            GameObject v = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            Destroy(v, particleDuration);
        }

        // 快捷播放一次性音效（无需手动创建 AudioSource）
        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, explosionVolume);

        Destroy(gameObject);
    }
}