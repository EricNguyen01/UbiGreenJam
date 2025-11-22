using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : CharacterComponentBase
{
    [Header("Components")]
    [SerializeField] public CharacterController characterController;
    private PlayerFloodDetector flood;

    [Header("General Movement Settings")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float deceleration = 12f;
    [SerializeField] private float airControl = 0.5f;
    [SerializeField] private float gravity = -16f;
    [SerializeField] private float jumpForce = 6f;

    [Header("Ground Check Settings")]
    [Tooltip("Which layers count as ground")]
    [SerializeField] private LayerMask groundMask;
    [Tooltip("Radius of the ground check sphere")]
    [SerializeField] private float groundCheckRadius = 0.25f;
    [Tooltip("How far below feet to check")]
    [SerializeField] private float groundCheckDistance = 0.15f;
    [Tooltip("Optional debug log")]
    [SerializeField] private bool debugGrounded = false;

    private bool isGrounded;

    private Vector3 velocity;
    private Vector3 moveVelocity;

    // -------- WATER VALUES (Tweakable) --------
    [Header("Water Movement Settings")]
    [SerializeField] private float waterMinSpeedMultiplier = 0.45f;
    [SerializeField] private float waterCurveExponent = 0.8f;
    [SerializeField] private float waterAirDrag = 0.25f;
    [SerializeField] private float submergedGravityMultiplier = 0.35f;
    [SerializeField] private Vector2 buoyancyJump = new Vector2(2.5f, 5f);

    private bool inWater = false;
    private float submergedAmount = 0f;
    // ------------------------------------------

    private void Awake()
    {
        if (!characterController)
            characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        FloodCheck();
        GroundCheck();

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 inputDir = transform.right * input.x + transform.forward * input.y;
        inputDir = Vector3.ClampMagnitude(inputDir, 1f);

        // SPEED
        float targetSpeed = walkSpeed;
        if (inWater)
        {
            float curve = Mathf.Pow(submergedAmount, waterCurveExponent);
            targetSpeed *= Mathf.Lerp(1f, waterMinSpeedMultiplier, curve);
        }

        // Accel / Decel smoothing
        if (inputDir.magnitude > 0.1f)
            moveVelocity = Vector3.Lerp(moveVelocity, inputDir * targetSpeed, acceleration * Time.deltaTime);
        else
            moveVelocity = Vector3.Lerp(moveVelocity, Vector3.zero, deceleration * Time.deltaTime);

        // JUMP
        if (Input.GetButtonDown("Jump"))
        {
            if (inWater)
            {
                float jumpUp = Mathf.Lerp(buoyancyJump.x, buoyancyJump.y, submergedAmount);
                velocity.y = jumpUp;
            }
            else if (isGrounded)
            {
                velocity.y = jumpForce;
            }
        }

        // GRAVITY
        if (inWater)
        {
            float waterG = Mathf.Lerp(gravity, gravity * submergedGravityMultiplier, submergedAmount);
            velocity.y += waterG * Time.deltaTime;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // AIR CONTROL
        if (!isGrounded)
        {
            Vector3 airMove = inputDir * targetSpeed * airControl;
            moveVelocity = Vector3.Lerp(moveVelocity, airMove, (acceleration / 2f) * Time.deltaTime);

            if (inWater)
                moveVelocity *= (1f - waterAirDrag);
        }

        // FINAL MOVE
        Vector3 finalMovement = moveVelocity + Vector3.up * velocity.y;
        characterController.Move(finalMovement * Time.deltaTime);
    }

    // --------------------------------------------------------
    private void GroundCheck()
    {
        // Compute feet position from CharacterController
        Vector3 feetPos = transform.position + characterController.center
                        - Vector3.up * (characterController.height * 0.5f);

        // Check slightly below feet
        Vector3 checkPos = feetPos + Vector3.down * groundCheckDistance;

        bool sphereHit = Physics.CheckSphere(checkPos, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);

        // Fallback to built-in grounded
        isGrounded = sphereHit || characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (debugGrounded)
            Debug.Log($"Grounded? {isGrounded} | sphereHit={sphereHit} | ccGrounded={characterController.isGrounded}");
    }

    // --------------------------------------------------------
    private void FloodCheck()
    {
        if (!flood) flood = GetComponent<PlayerFloodDetector>();
        if (!flood)
        {
            inWater = false;
            submergedAmount = 0f;
            return;
        }

        inWater = flood.isInWater;
        submergedAmount = flood.submergedAmount;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!characterController) characterController = GetComponent<CharacterController>();
        if (!characterController) return;

        Vector3 feetPos = transform.position + characterController.center
                        - Vector3.up * (characterController.height * 0.5f);
        Vector3 checkPos = feetPos + Vector3.down * groundCheckDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
    }
#endif
}
