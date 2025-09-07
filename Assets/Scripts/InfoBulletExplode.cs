using System.Collections;
using UnityEngine;

public class ExplosionVisualOnly : MonoBehaviour
{
    public float BulletLiveTime;
    public GameObject particlePrefab;
    public AudioClip explosionSound;
    public float particleDuration = 3f;

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

        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        // 发送消息，供其他脚本处理（可改为事件系统）
        SendMessage("OnExploded", SendMessageOptions.DontRequireReceiver);

        Destroy(gameObject);
    }
}