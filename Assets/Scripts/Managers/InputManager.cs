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

    public bool ShouldJump { get; set; }
    public bool ShouldRun { get; set; }
    public bool ShouldCrouch { get; set; }

    public bool ShouldUseSkill1 { get; set; }
    public bool ShouldUseSkill2 { get; set; }

    public bool MovementLocked { get; set; }
    public bool RotationLocked { get; set; }
    public bool AffectedByGravity { get; set; } = true;

    public Vector2 PrimaryFinger { get; private set; }
    public Vector2 SecondaryFinger { get; private set; }
    public bool SecondaryFingerTouched { get; private set; }

    public Vector2 CameraInput { get; set; }

    public enum ControlScheme { Any, Gamepad, KeyboardAndMouse, TouchScreen };
    [Header("Scheme")]
    public ControlScheme SelectedControlScheme = ControlScheme.Any;

    [Header("UI")]
    public Canvas TouchScreenUI = null;

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

        if (SelectedControlScheme == ControlScheme.Any || SystemInfo.deviceType == DeviceType.Handheld)
        {
            switch (SystemInfo.deviceType)
            {
                case DeviceType.Handheld:
                    SelectedControlScheme = ControlScheme.TouchScreen;
                    break;
                case DeviceType.Desktop:
                    SelectedControlScheme = ControlScheme.KeyboardAndMouse;
                    break;
                default:
                    SelectedControlScheme = ControlScheme.Gamepad;
                    break;
            }
        }

        if (SelectedControlScheme == ControlScheme.TouchScreen)
        {
            if (TouchScreenUI != null)
                TouchScreenUI.enabled = true;
        }
        else
        {
            if (TouchScreenUI != null)
                TouchScreenUI.enabled = false;
        }

        if (HideCursor)
        {
            Cursor.visible = HideCursor;
            Cursor.lockState = CursorLockMode.Locked;
        }

        inputActions = new PlayerInputActions();

        // Register input listeners
        inputActions.Gameplay.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        inputActions.Gameplay.Move.canceled += _ => MoveInput = Vector2.zero;

        inputActions.Gameplay.Jump.performed += _ => ShouldJump = true;
        inputActions.Gameplay.Jump.canceled += _ => ShouldJump = false;

        inputActions.Gameplay.Run.performed += _ => ShouldRun = !ShouldRun;
        inputActions.Gameplay.Crouch.performed += _ => ShouldCrouch = !ShouldCrouch;


        inputActions.Gameplay.Skill1.performed += _ => ShouldUseSkill1 = true;
        inputActions.Gameplay.Skill1.canceled += _ => ShouldUseSkill1 = false;

        inputActions.Gameplay.Skill2.performed += _ => ShouldUseSkill2 = true;
        inputActions.Gameplay.Skill2.canceled += _ => ShouldUseSkill2 = false;

        inputActions.Gameplay.Look.performed += ctx => CameraInput = ctx.ReadValue<Vector2>();

        inputActions.Gameplay.PrimaryFingerPosition.performed += ctx => PrimaryFinger = ctx.ReadValue<Vector2>();
        inputActions.Gameplay.SecondaryFingerPosition.performed += ctx => SecondaryFinger = ctx.ReadValue<Vector2>();
        inputActions.Gameplay.SecondaryFingerTouched.started += _ => SecondaryFingerTouched = true;
        inputActions.Gameplay.SecondaryFingerTouched.canceled += _ => SecondaryFingerTouched = false;
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
