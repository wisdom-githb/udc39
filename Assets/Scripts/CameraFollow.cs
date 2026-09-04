using UnityEngine;

public sealed class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 13f, -12f);
    [SerializeField, Min(0.01f)] private float smoothTime = 0.35f;
    [SerializeField, Min(0f)] private float maxFollowSpeed = 12f;

    private Vector3 followVelocity;
    private float fixedHeight;

    private void Awake()
    {
        fixedHeight = transform.position.y;
    }

    private void OnEnable()
    {
        followVelocity = Vector3.zero;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            fixedHeight,
            target.position.z + offset.z);
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref followVelocity,
            smoothTime,
            maxFollowSpeed,
            Time.deltaTime);
    }
}
