using UnityEngine;

public class MovingPlatformForward : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 10f;

    private Vector3 _dir = Vector3.forward;


    void Update()
    {
        transform.position += _dir * speed * Time.deltaTime;
    }
}
