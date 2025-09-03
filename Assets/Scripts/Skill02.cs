using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Skill02 : MonoBehaviour
{
    [Header("Prefab & Spawn")]
    [SerializeField] private GameObject callPopupPrefab; // 一定要拖“Project面板里的Prefab资产”进来
    [SerializeField, Min(1)] private int popupCount = 5;

    [Header("Spawn Area (Table Surface - XZ)")]
    [SerializeField, Min(0f)] private float areaWidth = 8f;   // X 方向
    [SerializeField, Min(0f)] private float areaDepth = 4f;   // Z 方向
    [SerializeField] private float spawnHeight = 0f;          // 距桌面高度（可为0贴面）

    [Header("Lifetime")]
    [SerializeField, Min(0f)] private float popupLifetime = 10f; // 实例的生存时间


    void Update()
    {
        if(Input.GetKeyUp(KeyCode.E))
        {
            SpawnCallPopups();
        }
    }



    public void SpawnCallPopups()
    {
        if (callPopupPrefab == null)
        {
            Debug.LogError("callPopupPrefab 为空：请从 Project 面板拖入预制体资产，而不是场景中的对象。");
            return;
        }

        for (int i = 0; i < popupCount; i++)
        {
            float randX = Random.Range(-areaWidth * 0.5f, areaWidth * 0.5f);
            float randZ = Random.Range(-areaDepth * 0.5f, areaDepth * 0.5f);
            Vector3 spawnPos = transform.position + new Vector3(randX, spawnHeight, randZ);

            // 生成“实例”
            GameObject instance = Instantiate(callPopupPrefab, spawnPos, Quaternion.identity);

            // 正确：销毁“实例”，而不是 callPopupPrefab
            if (popupLifetime > 0f)
            {
                Destroy(instance, popupLifetime);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 size = new Vector3(areaWidth, 0.05f, areaDepth);
        Gizmos.DrawWireCube(transform.position + new Vector3(0f, spawnHeight, 0f), size);
    }
}
