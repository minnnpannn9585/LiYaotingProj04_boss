using System.Collections;
using UnityEngine;

public class ExplosionVisualOnly : MonoBehaviour
{
    public float BulletLiveTime;
    public GameObject particlePrefab;

    //VfX
    public float particleDuration = 3f;
    public AudioClip explosionSound;
    public float explosionVolume = 1f;

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

        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, explosionVolume);

        Destroy(gameObject);
    }
}