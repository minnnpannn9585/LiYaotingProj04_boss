using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill02 : MonoBehaviour
{
    [Header("Prefab & Spawn")]
    [SerializeField] private GameObject goodPopupPrefab;   // 好的预制体
    [SerializeField] private GameObject badPopupPrefab;    // 坏的预制体
    [SerializeField, Min(0)] private int goodPopupCount = 3; // 好的生成数量
    [SerializeField, Min(0)] private int badPopupCount = 2;  // 坏的生成数量

    [Header("Spawn Area (Table Surface - XZ)")]
    [SerializeField, Min(0f)] private float areaWidth = 8f;   // X 方向
    [SerializeField, Min(0f)] private float areaDepth = 4f;   // Z 方向
    [SerializeField] private float spawnHeight = 0f;          // Y 偏移

    // 新增：用于 OverlapBox 检测的参数
    [Header("OverlapBox Check")]
    [SerializeField] private LayerMask blockerLayer; // 把预制体设置到该 Layer
    [SerializeField] private Vector3 boxHalfExtents = new Vector3(0.5f, 0.5f, 0.5f); // 检测盒半尺寸（按 X,Y,Z）
    [SerializeField, Min(1)] private int maxFindAttempts = 20; // 找位置重试次数

    [Header("Lifetime")]
    [Header("Cast")]
    [SerializeField, Min(0f)] private float maxCastDuration = 10f; // 协程等待的最大时长（秒）

    // 运行时管理
    private List<GameObject> spawnedInstances = new List<GameObject>();
    private int remainingGoodCount = 0;

    // 对外调用：二技能的协程接口，LevelManager 使用 StartCoroutine(skill02.CastSkill())
    public IEnumerator CastSkill()
    {
        // 触发一次生成
        SpawnCallPopups();

        // 等待所有 good 被收集，或超时
        float timer = 0f;
        while (remainingGoodCount > 0 && timer < maxCastDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // 超时或全部收集后，确保场上清空并结束协程
        DestroyAllSpawnedPopups();
        yield break;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            SpawnCallPopups();
        }
    }

    public void SpawnCallPopups()
    {
        if (goodPopupPrefab == null && badPopupPrefab == null)
        {
            Debug.LogError("goodPopupPrefab 和 badPopupPrefab 都为 null，请在 Inspector 指定至少一个预制体。");
            return;
        }

        // 清理旧数据
        spawnedInstances.Clear();

        // 实际生成的好预制体数量（如果 goodPrefab 为 null 则为 0）
        remainingGoodCount = (goodPopupPrefab != null) ? goodPopupCount : 0;

        // 生成好的预制体
        if (goodPopupPrefab != null)
        {
            for (int i = 0; i < goodPopupCount; i++)
            {
                Vector3? pos = TryFindFreePosition();
                if (pos == null)
                {
                    Debug.LogWarning("未能找到不重叠位置，仍将使用随机位置。");
                    pos = GetRandomSpawnPosition();
                }

                GameObject instance = Instantiate(goodPopupPrefab, pos.Value, Quaternion.identity);

                // 添加碰撞通知组件，告知这是“好”的预制体
                var collector = instance.AddComponent<PopupCollector>();
                collector.Setup(this, true);

                spawnedInstances.Add(instance);
            }
        }

        // 生成坏的预制体
        if (badPopupPrefab != null)
        {
            for (int i = 0; i < badPopupCount; i++)
            {
                Vector3? pos = TryFindFreePosition();
                if (pos == null)
                {
                    Debug.LogWarning("未能找到不重叠位置，仍将使用随机位置。");
                    pos = GetRandomSpawnPosition();
                }

                GameObject instance = Instantiate(badPopupPrefab, pos.Value, Quaternion.identity);

                // 添加碰撞通知组件，标记为“坏”的预制体（玩家碰到坏的不会触发全部销毁逻辑）
                var collector = instance.AddComponent<PopupCollector>();
                collector.Setup(this, false);

                spawnedInstances.Add(instance);
            }
        }
    }

    // 被 PopupCollector 调用：某个预制体被玩家碰到了
    public void NotifyCollected(GameObject obj, bool isGood)
    {
        // 如果是好的，减少计数
        if (isGood)
        {
            remainingGoodCount = Mathf.Max(0, remainingGoodCount - 1);
        }

        // 销毁被碰到的实例（防止重复触发，先移出列表）
        if (spawnedInstances.Contains(obj))
        {
            spawnedInstances.Remove(obj);
        }
        if (obj != null)
        {
            Destroy(obj);
        }

        // 如果所有好的都被玩家碰到，则销毁场上所有剩余生成的预制体
        if (remainingGoodCount <= 0)
        {
            DestroyAllSpawnedPopups();
        }
    }

    private void DestroyAllSpawnedPopups()
    {
        // 复制一份避免在循环中修改原列表
        var copy = new List<GameObject>(spawnedInstances);
        foreach (var go in copy)
        {
            if (go != null)
            {
                Destroy(go);
            }
        }
        spawnedInstances.Clear();
    }

    private Vector3? TryFindFreePosition()
    {
        for (int i = 0; i < maxFindAttempts; i++)
        {
            Vector3 candidate = GetRandomSpawnPosition();
            if (IsBoxAreaFree(candidate))
                return candidate;
        }
        return null;
    }

    private bool IsBoxAreaFree(Vector3 center)
    {
        // OverlapBox 使用半尺寸，包含 trigger，需要设置 QueryTriggerInteraction.Collide
        Collider[] hits = Physics.OverlapBox(center, boxHalfExtents, Quaternion.identity, blockerLayer, QueryTriggerInteraction.Collide);
        return hits.Length == 0;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float randX = Random.Range(-areaWidth * 0.5f, areaWidth * 0.5f);
        float randZ = Random.Range(-areaDepth * 0.5f, areaDepth * 0.5f);
        return transform.position + new Vector3(randX, spawnHeight, randZ);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 size = new Vector3(areaWidth, 0.05f, areaDepth);
        Gizmos.DrawWireCube(transform.position + new Vector3(0f, spawnHeight, 0f), size);
    }

}
