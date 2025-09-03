using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Skill02 : MonoBehaviour
{
    [Header("Prefab & Spawn")]
    [SerializeField] private GameObject callPopupPrefab; // һ��Ҫ�ϡ�Project������Prefab�ʲ�������
    [SerializeField, Min(1)] private int popupCount = 5;

    [Header("Spawn Area (Table Surface - XZ)")]
    [SerializeField, Min(0f)] private float areaWidth = 8f;   // X ����
    [SerializeField, Min(0f)] private float areaDepth = 4f;   // Z ����
    [SerializeField] private float spawnHeight = 0f;          // ������߶ȣ���Ϊ0���棩

    [Header("Lifetime")]
    [SerializeField, Min(0f)] private float popupLifetime = 10f; // ʵ��������ʱ��


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
            Debug.LogError("callPopupPrefab Ϊ�գ���� Project �������Ԥ�����ʲ��������ǳ����еĶ���");
            return;
        }

        for (int i = 0; i < popupCount; i++)
        {
            float randX = Random.Range(-areaWidth * 0.5f, areaWidth * 0.5f);
            float randZ = Random.Range(-areaDepth * 0.5f, areaDepth * 0.5f);
            Vector3 spawnPos = transform.position + new Vector3(randX, spawnHeight, randZ);

            // ���ɡ�ʵ����
            GameObject instance = Instantiate(callPopupPrefab, spawnPos, Quaternion.identity);

            // ��ȷ�����١�ʵ������������ callPopupPrefab
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
