using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : CharacterComponentBase
{
    [field: Header("Character Movement Components")]

    [field: SerializeField]
    public CharacterController characterController { get; private set; }

    [Header("Character Movement Settings")]

    [Header("Character Ground Check Settings")]

    [SerializeField]
    private Transform groundCheckTransform;

    [SerializeField]
    private LayerMask groundMask;

    [SerializeField]
    private float groundDistance = 0.4f;

    private bool isGrounded = true;

    private float speed = 12.0f;

    private float jumpHeight = 3.0f;

    private float gravity = -9.81f;

    private Vector3 velocity;

    // Mid-air control settings

    // How much control the player has while in the air (1 = full control, 0 = no control)
    private float airControlMultiplier = 0.5f;

    // How quickly horizontal velocity approaches the target while in the air
    private float airAcceleration = 10f;

    // Internal horizontal velocity used to smoothly blend ground/air control
    private Vector3 horizontalVelocity;

    private void Awake()
    {
        if (!characterController)
        {
            characterController = GetComponent<CharacterController>();
        }

        if (!characterController)
        {
            Debug.LogError($"Character {name}'s CharacterMovement component is missing its Character Controller component. " +
                           "One will be added but the character and its movement might not work correctly!");

            characterController = gameObject.AddComponent<CharacterController>();
        }

        if(!groundCheckTransform)
        {
            Vector3 bottomCenter;

            if (characterController)
            {
                bottomCenter = transform.position + characterController.center - new Vector3(0.0f, characterController.height / 2.0f, 0.0f);
            }
            else
            {
                bottomCenter = transform.position - Vector3.up * 1.56f;
            }

            GameObject go = new GameObject("CharacterGroundCheck");

            go.transform.position = bottomCenter;

            go.transform.SetParent(transform);

            groundCheckTransform = go.transform;
        }
        else
        {
            if(groundCheckTransform.parent != transform)
            {
                groundCheckTransform.SetParent(transform);
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!enabled) return;

        isGrounded = Physics.CheckSphere(groundCheckTransform.position, groundDistance, groundMask);

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
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

                isGrounded = false;
            }
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Move controller using combined horizontal and vertical velocities
        Vector3 totalMove = horizontalVelocity + new Vector3(0f, velocity.y, 0f);

        characterController.Move(totalMove * Time.deltaTime);
    }

    public override bool InitCharacterComponentFrom(CharacterBase character)
    {
        if (!base.InitCharacterComponentFrom(character)) return false;
        
        speed = character.characterSOData.speed;

        jumpHeight = character.characterSOData.jumpHeight;

        airControlMultiplier = character.characterSOData.airControlMultiplier;

        airAcceleration = character.characterSOData.airAcceleration;

        gravity = character.characterSOData.gravity;

        return true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (groundCheckTransform)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawSphere(groundCheckTransform.transform.position, groundDistance);
        }
    }
#endif
}