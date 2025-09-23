using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Button : MonoBehaviour
{
    bool canDefeat;

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            canDefeat = true;
        }

    }

    void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            canDefeat = false;
        }

    }

    void Update()
    {

        if (canDefeat && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("按下E键，触发按钮效果");
            SceneManager.LoadScene(2);

        }


    }

}
