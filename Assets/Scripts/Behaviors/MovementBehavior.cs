using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBehavior : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;
    public float crouchSpeed = 1.5f;
    public float speedSmoothTime = 0.2f;

    [Header("Look Parameters")]
    public Transform cameraTransform;
    public float turnSmoothTime = 0.2f;

    [Header("Jumping Parameters")]
    public float jumpForce = 8.0f;
    public float gravity = -9.81f;
    public LayerMask groundLayer;

    [Header("Collision Parameters")]
    [Range(0.05f, 1.0f)]
    public float groundCheckRadius = 0.1f;

    [Header("Debug")]
    public Color groundCheckColor = Color.magenta;
    public bool drawGroundCheck = false;

    private CharacterController characterController;
    private Animator animator;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 directionInput;

    private float speed;
    private float speedSmoothVelocity;
    private float turnSmoothVelocity;
    private bool hasDoubledJumped = false;
    private bool isGrounded = false;

    private int lastGroundedFrame = 0;

    // Animator Hashes
    private readonly int moveInputMagHash = Animator.StringToHash("MoveInputMag");
    private readonly int moveInputXHash = Animator.StringToHash("MoveInputX");
    private readonly int moveInputYHash = Animator.StringToHash("MoveInputY");
    private readonly int isRunningHash = Animator.StringToHash("IsRunning");
    private readonly int isCrouchingHash = Animator.StringToHash("IsCrouching");
    private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
    private readonly int doubleJumpedHash = Animator.StringToHash("DoubleJumped");
    private readonly int JumpHash = Animator.StringToHash("Jump");
    private readonly int MoveInputDot = Animator.StringToHash("MoveInputDot");

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Reserved for future use.
    }

    public void HandleMovement()
    {
        PerformGroundCheck();
        HandleDirection();
        HandleSpeedAndMovement();
        HandleJumping();
        HandleAnimations();
    }

    private void PerformGroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayer);
    }


    private void HandleDirection()
    {
        Vector2 moveInput = InputManager.Instance.MoveInput;
        directionInput = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        if (moveInput.magnitude != 0)
        {
            if (!InputManager.Instance.RotationLocked)
            {
                float targetRotation = Mathf.Atan2(directionInput.x, directionInput.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            }
        }
        else
        {
            InputManager.Instance.ShouldRun = false;
        }
    }


    private void HandleSpeedAndMovement()
    {
        Vector2 moveInput = InputManager.Instance.MoveInput;
        float targetSpeed = 0.0f;

        if (InputManager.Instance.ShouldRun)
        {
            targetSpeed = runSpeed;
            InputManager.Instance.ShouldCrouch = false;
        }
        else
        {
            targetSpeed = isGrounded && InputManager.Instance.ShouldCrouch ? crouchSpeed : walkSpeed;
        }

        speed = Mathf.SmoothDamp(speed, targetSpeed * moveInput.magnitude, ref speedSmoothVelocity, speedSmoothTime);

        if (moveInput.magnitude == 0)
            InputManager.Instance.ShouldRun = false;

        characterController.Move(speed * Time.deltaTime * transform.forward);
    }

    private void HandleJumping()
    {
        if ((isGrounded || !hasDoubledJumped) && InputManager.Instance.ShouldJump)
        {
            if (!isGrounded)
            {
                hasDoubledJumped = true;
                animator.SetBool(doubleJumpedHash, true);
            }
            else
            {
                animator.SetTrigger(JumpHash);
            }

            moveDirection.y = jumpForce;
            InputManager.Instance.ShouldJump = false;
            lastGroundedFrame = 0;
        }
        else if (isGrounded)
        {
            if (lastGroundedFrame > 1)
            {
                moveDirection.y = 0.0f;
            }


            animator.ResetTrigger(JumpHash);
            animator.SetBool(doubleJumpedHash, false);

            lastGroundedFrame++;
        }

        if (InputManager.Instance.AffectedByGravity)
            moveDirection.y += gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleAnimations()
    {
        Vector2 moveInput = InputManager.Instance.MoveInput;

        animator.SetFloat(speedHash, speed);
        animator.SetFloat(verticalSpeedHash, moveDirection.y);
        animator.SetFloat(moveInputMagHash, moveInput.magnitude);
        animator.SetFloat(moveInputXHash, moveInput.x);
        animator.SetFloat(moveInputYHash, moveInput.y);
        animator.SetBool(isRunningHash, InputManager.Instance.ShouldRun);
        animator.SetBool(isCrouchingHash, InputManager.Instance.ShouldCrouch);
        animator.SetBool(isGroundedHash, isGrounded);
        animator.SetFloat(MoveInputDot, GetMoveInputDot());
    }

    private float GetMoveInputDot()
    {
        Vector3 forward = transform.forward;
        Vector2 moveInput = InputManager.Instance.MoveInput;

        Vector3 cameraDirectionInput = cameraTransform.TransformDirection(directionInput);
        cameraDirectionInput.y = 0.0f;
        cameraDirectionInput.Normalize();

        return Vector3.Dot(forward, cameraDirectionInput);

    }

    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? groundCheckColor : Color.red;

        if (characterController != null)
        {
            if (drawGroundCheck)
            {
                Gizmos.DrawWireSphere(transform.position, groundCheckRadius);

                for (float i = -characterController.radius; i < characterController.radius; i += groundCheckRadius)
                {
                    Gizmos.DrawLine(transform.position - Vector3.right * i, transform.position - Vector3.right * i + Vector3.down * groundCheckRadius);
                }
            }


            // Combine the player's rotation with the camera's rotation
            float cameraAngle = transform.eulerAngles.y + cameraTransform.eulerAngles.y;

            // Convert the angle to a vector
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0;
            cameraForward.Normalize();

            // Draw a line from the player in the direction the camera is facing
            Gizmos.color = Color.blue;  // Change color for this specific line, if you want
            Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + cameraForward * 2.0f);
            Gizmos.DrawWireSphere(transform.position + Vector3.up + cameraForward * 2.0f, 0.15f);

            Gizmos.color = Color.magenta;  // Change color for this specific line, if you want
            Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + transform.forward * 2.0f);
            Gizmos.DrawWireSphere(transform.position + Vector3.up + transform.forward * 2.0f, 0.15f);


            Vector3 cameraDirectionInput = cameraTransform.TransformDirection(directionInput);
            cameraDirectionInput.y = 0.0f;
            cameraDirectionInput.Normalize();

            Gizmos.color = Color.red;  // Change color for this specific line, if you want
            Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + cameraDirectionInput * directionInput.magnitude * 2.0f);
            Gizmos.DrawWireSphere(transform.position + Vector3.up + cameraDirectionInput * directionInput.magnitude * 2.0f, 0.15f);
        }

    }
}

