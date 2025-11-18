using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : CharacterComponentBase
{
    [Header("Character Movement Components")]

    [SerializeField]
    private CharacterController characterController;

    [SerializeField]
    private Rigidbody characterRigidbody;

    [Header("Character Movement Settings")]

    [Header("Character Ground Check Settings")]

    [SerializeField]
    private Transform groundCheckTransform;

    private Vector3 groundCheckPos = Vector3.zero;

    [SerializeField]
    private LayerMask groundMask;

    [SerializeField]
    private float groundDistance = 0.4f;

    private bool isGrounded = true;

    private float speed = 12.0f;

    private float jumpHeight = 3.0f;

    private Vector3 velocity;

    // Mid-air control settings
    // How much control the player has while in the air (1 = full control, 0 = no control)
    public float airControlMultiplier = 0.5f;

    // How quickly horizontal velocity approaches the target while in the air
    public float airAcceleration = 10f;

    // Internal horizontal velocity used to smoothly blend ground/air control
    Vector3 horizontalVelocity;

    private void Awake()
    {
        if (!characterController)
        {
            TryGetComponent<CharacterController>(out characterController);
        }

        if (!characterController)
        {
            Debug.LogError($"Character {name}'s CharacterMovement component is missing its Character Controller component. " +
                           "One will be added but the character and its movement might not work correctly!");

            characterController = gameObject.AddComponent<CharacterController>();
        }

        if (!characterRigidbody)
        {
            TryGetComponent<Rigidbody>(out characterRigidbody);

            if (!characterRigidbody)
            {
                characterRigidbody = gameObject.AddComponent<Rigidbody>();
            }
        }

        if(!groundCheckTransform)
        {
            if (characterController)
            {
                Vector3 bottomCenter = transform.position + characterController.center - new Vector3(0.0f, characterController.height / 2.0f, 0.0f);

                GameObject go = new GameObject("CharacterGroundCheck");

                go.transform.position = bottomCenter;

                go.transform.SetParent(transform);

                groundCheckTransform = go.transform;

                groundCheckPos = go.transform.position;
            }
            else
            {
                groundCheckPos = transform.position - Vector3.up * 1.5f;
            }
        }
        else
        {
            if(groundCheckTransform.parent != transform)
            {
                groundCheckTransform.SetParent(transform);
            }

            groundCheckPos = groundCheckTransform.position;
        }
    }

    private void Start()
    {
        if (!characterUsingComponent || !characterUsingComponent.characterSOData)
        {
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!enabled) return;

        isGrounded = Physics.CheckSphere(groundCheckPos, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 inputDirection = transform.right * x + transform.forward * z;

        // Determine desired horizontal velocity
        Vector3 desiredHorizontal;

        if (isGrounded)
        {
            // Full control on ground
            desiredHorizontal = inputDirection * speed;

            // Immediately match horizontal velocity when grounded for responsive control
            horizontalVelocity = desiredHorizontal;
        }
        else
        {
            // Reduced control while in air
            desiredHorizontal = inputDirection * speed * airControlMultiplier;

            // Smoothly move current horizontal velocity toward the desired value

            // Use an exponential approach to make the feel frame-rate independent
            float t = 1f - Mathf.Exp(-airAcceleration * Time.deltaTime);

            horizontalVelocity = Vector3.Lerp(horizontalVelocity, desiredHorizontal, t);
        }

        // Jumping
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            }
        }

        // Apply gravity
        velocity.y += Physics.gravity.y * Time.deltaTime;

        // Move controller using combined horizontal and vertical velocities
        Vector3 totalMove = horizontalVelocity + new Vector3(0f, velocity.y, 0f);

        characterController.Move(totalMove * Time.deltaTime);
    }

    private bool IsGrounded()
    {

        return false;
    }

    public override bool InitCharacterComponentFrom(CharacterBase character)
    {
        if (!base.InitCharacterComponentFrom(character)) return false;

        speed = character.characterSOData.speed;

        jumpHeight = character.characterSOData.jumpHeight;

        return true;
    }
}