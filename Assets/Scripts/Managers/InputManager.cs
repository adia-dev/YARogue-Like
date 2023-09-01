using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance; // Singleton Instance

    private PlayerInputActions inputActions;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public bool ShouldJump { get;  set; }
    public bool ShouldRun { get;  set; }
    public bool ShouldCrouch { get;  set; }


    public bool MovementLocked { get; set; }
    public bool RotationLocked { get; set; }

    [Header("Debug")]
    public bool HideCursor = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if(HideCursor) {
            Cursor.visible = HideCursor;
            Cursor.lockState = CursorLockMode.Locked;
        }

        inputActions = new PlayerInputActions();

        // Register input listeners
        inputActions.Gameplay.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        inputActions.Gameplay.Move.canceled += ctx => MoveInput = Vector2.zero;

        inputActions.Gameplay.Jump.performed += ctx => ShouldJump = true;
        inputActions.Gameplay.Jump.canceled += ctx => ShouldJump = false;

        inputActions.Gameplay.Run.performed += ctx => ShouldRun = true;
        inputActions.Gameplay.Run.canceled += ctx => ShouldRun = false;

        inputActions.Gameplay.Crouch.performed += ctx => ShouldCrouch = true;
        inputActions.Gameplay.Crouch.canceled += ctx => ShouldCrouch = false;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
