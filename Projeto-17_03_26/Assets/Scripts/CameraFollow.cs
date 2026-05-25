using UnityEngine;

/// <summary>
/// Simple smooth follow camera. Attach to the Main Camera and either assign a target
/// in the inspector or the script will try to find a GameObject tagged 'Player'.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 5f, -8f);
    public float smoothTime = 0.12f;

    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        if (target == null)
        {
            var go = GameObject.FindWithTag("Player");
            if (go != null) target = go.transform;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 desired = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
        transform.LookAt(target.position + Vector3.up * 1.2f);
    }
}

