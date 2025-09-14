using System.Collections;
using UnityEngine;

public class WarningCircle : MonoBehaviour
{
    public GameObject warningPrefab;      // 必填：预警预制体
    public float preWarningDelay = 2f;    // 停止后等待秒数
    public float warningDuration;    // 预警存在时长，<=0 不自动销毁
    public float velocityThreshold = 0.05f; // 速度阈值，视为停止
    public Vector3 spawnOffset = new Vector3(0f, 0f, 0f);
    
    float startTime = 0.2f;
    bool spawned = false;

    Rigidbody rb;
    bool timerStarted = false;
    Vector3 stoppedPosition; // 新增：记录停止位置

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (timerStarted || rb == null) return;
        
        startTime -= Time.deltaTime;
        if (startTime <= 0)
        {
            spawned = true;
        }

        float thrSq = velocityThreshold * velocityThreshold;
        if (rb.velocity.sqrMagnitude <= thrSq && spawned)
        {
            timerStarted = true;
            stoppedPosition = transform.position; // 记录停止位置
            StartCoroutine(ShowWarningAfterDelay());
        }
    }

    IEnumerator ShowWarningAfterDelay()
    {
        yield return new WaitForSeconds(preWarningDelay);

        if (warningPrefab == null) yield break;

        // 将预警实例化为子对象，保证子弹爆炸时预警一并销毁
        GameObject w = Instantiate(warningPrefab, stoppedPosition + spawnOffset, Quaternion.identity, transform);
        if (warningDuration > 0f)
            Destroy(w, warningDuration);
    }
}