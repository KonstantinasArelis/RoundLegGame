using UnityEngine;

public class Follower : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = Vector3.zero;

    void FixedUpdate()
    {
        transform.position = target.position + offset;
    }
}
