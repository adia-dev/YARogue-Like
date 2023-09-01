using System;
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
    public float groundCheckRadius = 0.1f;

    [Header("Debug")]
    public Color groundCheckColor = Color.magenta;


    private CharacterController characterController;
    private Animator animator;

    private Vector3 moveDirection = Vector3.zero;

    float speed;
    float speedSmoothVelocity;
    float turnSmoothVelocity;

    bool hasDoubledJumped = false;
    bool isGrounded = false;

    int lastGroundedFrame = 0;

    // Animator Hashes
    int moveInputMagHash = Animator.StringToHash("MoveInputMag");
    int moveInputXHash = Animator.StringToHash("MoveInputX");
    int moveInputYHash = Animator.StringToHash("MoveInputY");
    int isRunningHash = Animator.StringToHash("IsRunning");
    int isCrouchingHash = Animator.StringToHash("IsCrouching");
    int isGroundedHash = Animator.StringToHash("IsGrounded");
    int speedHash = Animator.StringToHash("Speed");
    int verticalSpeedHash = Animator.StringToHash("VerticalSpeed");
    int doubleJumpedHash = Animator.StringToHash("DoubleJumped");
    int JumpHash = Animator.StringToHash("Jump");

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HandleMovement()
    {
        InputManager inputManagerInstance = InputManager.Instance;

        // Ground check
        isGrounded = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayer);
        animator.SetBool(isGroundedHash, isGrounded);

        // Get Input
        Vector2 moveInput = inputManagerInstance.MoveInput;
        Vector3 direction = new(moveInput.x, 0f, moveInput.y);

        // Handle movement direction based on camera
        Vector3 move = (cameraTransform.forward * direction.z + cameraTransform.right * direction.x) * (inputManagerInstance.MovementLocked ? 0 : 1);
        move.y = 0f;

        if (moveInput.magnitude != 0 && !inputManagerInstance.RotationLocked)
        {
            float targetRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }


        // Handle speed
        float targetSpeed = walkSpeed;
        if (inputManagerInstance.ShouldRun)
        {
            targetSpeed = runSpeed;
        }

        if (isGrounded && inputManagerInstance.ShouldCrouch)
        {
            targetSpeed = crouchSpeed;
        }

        if (isGrounded)
        {
            hasDoubledJumped = false;
            animator.SetBool(doubleJumpedHash, false);
            animator.SetBool(JumpHash, false);

            if(lastGroundedFrame > 1)
                moveDirection.y = 0.0f;

            lastGroundedFrame++;
        }else {
            lastGroundedFrame = 0;
        }

        speed = Mathf.SmoothDamp(speed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        // Move the character
        characterController.Move(speed * Time.deltaTime * move);

        // Handle Animations
        animator.SetFloat(speedHash, speed);
        animator.SetFloat(verticalSpeedHash, moveDirection.y);
        animator.SetFloat(moveInputMagHash, moveInput.magnitude);
        animator.SetFloat(moveInputXHash, moveInput.x);
        animator.SetFloat(moveInputYHash, moveInput.y);
        animator.SetBool(isRunningHash, inputManagerInstance.ShouldRun);
        animator.SetBool(isCrouchingHash, inputManagerInstance.ShouldCrouch);

        // Handle Jumping
        if ((isGrounded || !hasDoubledJumped) && inputManagerInstance.ShouldJump)
        {
        
            animator.SetBool(JumpHash, true);

            if (!isGrounded) { 
                hasDoubledJumped = true;
                animator.SetBool(doubleJumpedHash, true);
                moveDirection.y = jumpForce;
            } else { 
                moveDirection.y = jumpForce;
            }

            lastGroundedFrame = 0;
            inputManagerInstance.ShouldJump = false;
        }

        // Apply gravity
        moveDirection.y += gravity * Time.deltaTime;

        // Final Move
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? groundCheckColor : Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }
}
