using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Refs")]
    public RandomXSpawner spawnPoint;   // 出射口（挂了 RandomXSpawner 的空物体）
    public GameObject platformPrefab;   // 平台预制体（已挂 MovingPlatformForward）

    [Header("Timing")]
    public float spawnInterval = 1.0f;
    private float timer;

    [Header("Platform Motion")]
    public float platformSpeed = 10f;
    public bool useHorizontalDirection = true; // 与平台脚本保持一致

    [Header("Platform Lifetime")]
    [Tooltip("<=0 表示不销毁")]
    public float platformLifetime = 10f;
 
    void Update()
    {
        if (spawnPoint == null || platformPrefab == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer -= spawnInterval;
            SpawnOnce();
        }
    }

    void SpawnOnce()
    {
        // 1) 随机位置
        Vector3 pos = spawnPoint.GetSpawnPosition();

        // 2) 方向：用 SpawnPoint 的 forward（可选压平）
        Vector3 dir = spawnPoint.transform.forward;
        if (useHorizontalDirection) dir.y = 0f;
        if (dir.sqrMagnitude < 1e-6f) dir = Vector3.forward;
        dir.Normalize();

        // 3) 生成并初始化移动
        GameObject platform = Instantiate(platformPrefab, pos, Quaternion.identity);

        var mover = platform.GetComponent<MovingPlatformForward>();
        if (mover != null)
        {
            mover.useHorizontalDirection = useHorizontalDirection;
            mover.Initialize(dir, platformSpeed);
        }

        // 4) 可选：生存时间
        if (platformLifetime > 0f)
            Destroy(platform, platformLifetime);
    }
}
