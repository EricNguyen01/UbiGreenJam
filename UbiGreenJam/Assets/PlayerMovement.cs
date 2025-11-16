using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;

    public float gravity = -9.81f;

    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;

    // Mid-air control settings
    // How much control the player has while in the air (1 = full control, 0 = no control)
    public float airControlMultiplier = 0.5f;
    // How quickly horizontal velocity approaches the target while in the air
    public float airAcceleration = 10f;
    // Internal horizontal velocity used to smoothly blend ground/air control
    Vector3 horizontalVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

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
            }
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Move controller using combined horizontal and vertical velocities
        Vector3 totalMove = horizontalVelocity + new Vector3(0f, velocity.y, 0f);
        controller.Move(totalMove * Time.deltaTime);
    }
}