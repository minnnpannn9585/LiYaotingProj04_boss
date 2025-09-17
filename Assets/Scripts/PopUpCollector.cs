using UnityEngine;

public class PopupCollector : MonoBehaviour
{
    [Header("拾取音效")]
    public AudioClip collectSfx;
    [Range(0f, 1f)] public float sfxVolume = 1.0f;

    private Skill02 manager;
    private bool isGood;
    private bool collected;

    // 由 Skill02 在实例化后立即调用
    public void Setup(Skill02 mgr, bool goodFlag)
    {
        manager = mgr;
        isGood = goodFlag;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;
        if (manager == null) return;

        // 播放音效
        if (collectSfx != null)
        {
            AudioSource.PlayClipAtPoint(collectSfx, transform.position, sfxVolume);
        }

        collected = true;
        manager.NotifyCollected(gameObject, isGood);
    }
}