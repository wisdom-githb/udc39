using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public sealed class SphereController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 5.5f;

    [Header("Jump")]
    [SerializeField, Min(0f)] private float jumpVelocity = 12f;
    [SerializeField, Min(0.01f)] private float groundCheckDistance = 0.65f;
    [SerializeField, Min(1f)] private float riseGravityMultiplier = 2.6f;
    [SerializeField, Min(1f)] private float fallGravityMultiplier = 3.5f;

    [Header("Dash")]
    [SerializeField, Min(0f)] private float dashDistance = 6f;
    [SerializeField, Min(0.01f)] private float dashDuration = 0.18f;
    [SerializeField, Min(0f)] private float dashCooldown = 0.25f;

    private Rigidbody body;
    private Vector3 moveDirection;
    private Vector3 dashDirection;
    private float dashTimeRemaining;
    private float nextDashTime;
    private bool jumpRequested;

    private bool IsDashing => dashTimeRemaining > 0f;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.constraints = RigidbodyConstraints.FreezeRotation;
        body.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            moveDirection = Vector3.zero;
            return;
        }

        float horizontal = (keyboard.dKey.isPressed ? 1f : 0f) -
                           (keyboard.aKey.isPressed ? 1f : 0f);
        float vertical = (keyboard.wKey.isPressed ? 1f : 0f) -
                         (keyboard.sKey.isPressed ? 1f : 0f);

        moveDirection = Vector3.ClampMagnitude(new Vector3(horizontal, 0f, vertical), 1f);

        if (keyboard.spaceKey.wasPressedThisFrame && IsGrounded())
        {
            jumpRequested = true;
        }

        if (!IsDashing &&
            Time.time >= nextDashTime &&
            moveDirection.sqrMagnitude > 0.01f &&
            keyboard.leftShiftKey.wasPressedThisFrame)
        {
            dashDirection = moveDirection.normalized;
            dashTimeRemaining = dashDuration;
        }
    }

    private void FixedUpdate()
    {
        if (jumpRequested)
        {
            Vector3 velocity = body.linearVelocity;
            body.linearVelocity = new Vector3(velocity.x, jumpVelocity, velocity.z);
            jumpRequested = false;
        }

        Vector3 horizontalVelocity;

        if (IsDashing)
        {
            float dashSpeed = dashDistance / dashDuration;
            horizontalVelocity = dashDirection * dashSpeed;
            dashTimeRemaining -= Time.fixedDeltaTime;

            if (dashTimeRemaining <= 0f)
            {
                dashTimeRemaining = 0f;
                nextDashTime = Time.time + dashCooldown;
            }
        }
        else
        {
            horizontalVelocity = moveDirection * moveSpeed;
        }

        Vector3 currentVelocity = body.linearVelocity;

        if (!IsGrounded())
        {
            float gravityMultiplier = currentVelocity.y > 0f
                ? riseGravityMultiplier
                : fallGravityMultiplier;
            currentVelocity.y += Physics.gravity.y * (gravityMultiplier - 1f) * Time.fixedDeltaTime;
        }

        body.linearVelocity = new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.z);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(
            body.position,
            Vector3.down,
            groundCheckDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore);
    }

    public void ResetMotion()
    {
        moveDirection = Vector3.zero;
        dashDirection = Vector3.zero;
        dashTimeRemaining = 0f;
        nextDashTime = 0f;
        jumpRequested = false;

        if (body != null)
        {
            body.linearVelocity = Vector3.zero;
        }
    }
}
