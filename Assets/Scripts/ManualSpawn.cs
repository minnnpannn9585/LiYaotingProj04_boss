using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualSpawn : MonoBehaviour
{
    public GameObject[] platforms;
    int index = 0;
    public float cooldown = 2f;
    float timer = 0f;
    bool spawnEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        platforms = new GameObject[transform.childCount];
        //print("111");
        //print(transform.childCount);
        for (int j = 0; j < transform.childCount; j++)
        {
            //print("222");
            Transform child = transform.GetChild(j);
            platforms[j] = child.gameObject;
        }
    }

    void Update()
    {
        if (spawnEnd)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= cooldown)
        {
            platforms[index].SetActive(true);
            index++;
            if (index >= platforms.Length) {
                spawnEnd = true;
            };
            timer = 0f;
        }
    }
}
