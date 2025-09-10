using UnityEngine;

public class PopupCollector : MonoBehaviour
{
    [Tooltip("由 Skill02 在运行时引用，也可在 Inspector 手动拖入")]
    public Skill02 manager;

    [Tooltip("是否为好的预制体，可在 Inspector 设置")]
    [SerializeField] private bool isGood = true;

    // 新增：供 Skill02 在运行时设置 manager 和 isGood
    public void Setup(Skill02 mgr, bool isGood)
    {
        this.manager = mgr;
        this.isGood = isGood;
    }

    // 如果 manager 在运行时未设置，可以尝试自动寻找（可选）
    private void Awake()
    {
        if (manager == null)
        {
            manager = FindObjectOfType<Skill02>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (manager == null) return;
        if (other.CompareTag("Player"))
        {
            manager.NotifyCollected(gameObject, isGood);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (manager == null) return;
        if (collision.collider.CompareTag("Player"))
        {
            manager.NotifyCollected(gameObject, isGood);
        }
    }

    private void OnDestroy()
    {
        // 防止引用残留
        manager = null;
    }
}