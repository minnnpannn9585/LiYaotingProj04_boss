using System.Collections;
using UnityEngine;

public class ExplosionVisualOnly : MonoBehaviour
{
    public float BulletLiveTime;
    public GameObject particlePrefab;
    public float particleDuration = 3f;

    void Start()
    {
        StartCoroutine(DoVisualAfterDelay());
    }

    IEnumerator DoVisualAfterDelay()
    {
        yield return new WaitForSeconds(BulletLiveTime);
        if (particlePrefab != null)
        {
            GameObject v = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            Destroy(v, particleDuration);
        }

        Destroy(gameObject);
    }
}