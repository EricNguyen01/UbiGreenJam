using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : CharacterComponentBase
{
    [Header("Character Movement Components")]
    [SerializeField] public CharacterController characterController;

    [Header("Ground Settings")]
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance = 0.4f;

    private bool isGrounded = true;

    private float speed = 12f;
    private float jumpHeight = 3f;
    private float gravity = -9.81f;

    private Vector3 velocity;
    private float airControlMultiplier = 0.5f;
    private float airAcceleration = 10f;
    private Vector3 horizontalVelocity;

    // -------------------------------
    // WATER / FLOOD MOVEMENT PARAMETERS
    // -------------------------------
    [Header("Water Movement Settings")]
    [SerializeField] private float minWaterSpeedMultiplier = 0.4f;
    [SerializeField] private float waterSlowCurveExponent = 0.7f;
    [SerializeField] private float waterAirDrag = 0.3f;
    [SerializeField] private float submergedGravityMultiplier = 0.3f;
    [SerializeField] private Vector2 underwaterJumpRange = new Vector2(2f, 5f);

    // Internal water state
    private PlayerFloodDetector flood;
    private bool inWater = false;
    private float submergedAmount = 0f;

    // ---------------------------------------------------
    private void Awake()
    {
        // CharacterController setup
        if (!characterController)
            characterController = GetComponent<CharacterController>();

        // Create groundCheckTransform if missing
        if (!groundCheckTransform)
        {
            Vector3 bottomCenter = transform.position + characterController.center
                                  - new Vector3(0, characterController.height / 2f, 0);

            GameObject go = new GameObject("CharacterGroundCheck");
            go.transform.SetParent(transform);
            go.transform.position = bottomCenter;

            groundCheckTransform = go.transform;
        }
    }

    // ---------------------------------------------------
    private void Update()
    {
        if (!enabled) return;

        // FLOOD CHECK
        if (!flood) flood = GetComponent<PlayerFloodDetector>();
        if (flood)
        {
            inWater = flood.isInWater;
            submergedAmount = flood.submergedAmount;
        }
        else
        {
            inWater = false;
            submergedAmount = 0f;
        }

        // GROUND CHECK
        isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        // INPUT
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 inputDirection = transform.right * x + transform.forward * z;

        // WATER SLOWDOWN
        float slowCurve = Mathf.Pow(submergedAmount, waterSlowCurveExponent);
        float waterSpeedFactor = Mathf.Lerp(1f, minWaterSpeedMultiplier, slowCurve);
        float finalSpeed = inWater ? (speed * waterSpeedFactor) : speed;

        // HORIZONTAL MOVEMENT
        Vector3 desiredHorizontal;

        if (isGrounded)
        {
            desiredHorizontal = inputDirection * finalSpeed;
            horizontalVelocity = desiredHorizontal;
        }
        else
        {
            desiredHorizontal = inputDirection * finalSpeed * airControlMultiplier;

            float t = 1f - Mathf.Exp(-airAcceleration * Time.deltaTime);
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, desiredHorizontal, t);

            if (inWater)
                horizontalVelocity *= (1f - waterAirDrag);
        }

        // JUMP / BUOYANCY
        if (Input.GetButtonDown("Jump"))
        {
            if (inWater)
            {
                velocity.y = Mathf.Lerp(underwaterJumpRange.x, underwaterJumpRange.y, submergedAmount);
                isGrounded = false;
            }
            else if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                isGrounded = false;
            }
        }

        // GRAVITY
        if (inWater)
        {
            float waterGravity = Mathf.Lerp(gravity, gravity * submergedGravityMultiplier, submergedAmount);
            velocity.y += waterGravity * Time.deltaTime;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        Vector3 totalMove = horizontalVelocity + new Vector3(0, velocity.y, 0);
        characterController.Move(totalMove * Time.deltaTime);
    }
}
