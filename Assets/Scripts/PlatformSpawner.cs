using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Refs")]
    public RandomXSpawner spawnPoint;   // ����ڣ����� RandomXSpawner �Ŀ����壩
    public GameObject platformPrefab;   // ƽ̨Ԥ���壨�ѹ� MovingPlatformForward��

    [Header("Timing")]
    public float spawnInterval = 1.0f;
    private float timer;

    [Header("Platform Motion")]
    public float platformSpeed = 10f;
    public bool useHorizontalDirection = true; // ��ƽ̨�ű�����һ��

    [Header("Platform Lifetime")]
    [Tooltip("<=0 ��ʾ������")]
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
        // 1) ���λ��
        Vector3 pos = spawnPoint.GetSpawnPosition();

        // 2) ������ SpawnPoint �� forward����ѡѹƽ��
        Vector3 dir = spawnPoint.transform.forward;
        if (useHorizontalDirection) dir.y = 0f;
        if (dir.sqrMagnitude < 1e-6f) dir = Vector3.forward;
        dir.Normalize();

        // 3) ���ɲ���ʼ���ƶ�
        GameObject platform = Instantiate(platformPrefab, pos, Quaternion.identity);

        var mover = platform.GetComponent<MovingPlatformForward>();
        if (mover != null)
        {
            mover.useHorizontalDirection = useHorizontalDirection;
            mover.Initialize(dir, platformSpeed);
        }

        // 4) ��ѡ������ʱ��
        if (platformLifetime > 0f)
            Destroy(platform, platformLifetime);
    }
}
