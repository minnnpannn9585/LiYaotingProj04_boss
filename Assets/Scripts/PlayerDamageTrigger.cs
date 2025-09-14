using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageTrigger : MonoBehaviour
{
    [HideInInspector]
    public PlayerHealth playerHealth;
    public GameObject vfxPrefab;
    private void Start()
    {
        playerHealth = transform.parent.GetComponent<PlayerHealth>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Damage")
        {
            playerHealth.TakeDamage(10);
            Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        }
        
    }
}
