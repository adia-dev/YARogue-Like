using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CameraController : MonoBehaviour
{
    CinemachineFreeLook freeLookCamera;
    CinemachineInputProvider inputProvider;

    public float zoomSpeed = 10f;
    public float minZoom = 10f;
    public float maxZoom = 60f;
    public float zoomLerpSpeed = 2f; // This controls the smoothing speed

    private float targetZoomLevel;
    // To store initial distance between fingers
    private float initialPinchDistance;
    private bool isPinching = false;

    InputManager inputManagerInstance => InputManager.Instance;

    void Start()
    {
        freeLookCamera = GetComponent<CinemachineFreeLook>();
        inputProvider = GetComponent<CinemachineInputProvider>();

        if (freeLookCamera)
        {
            targetZoomLevel = freeLookCamera.m_Lens.FieldOfView;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleCameraZoom();
    }

    private void HandleCameraZoom()
    {
        float zoomDelta = 0.0f;

        if (inputManagerInstance.SecondaryFingerTouched && inputManagerInstance.MoveInput.magnitude == 0)
        {
            if (!isPinching)
            {
                initialPinchDistance = Vector2.Distance(inputManagerInstance.PrimaryFinger, inputManagerInstance.SecondaryFinger);
                DisableCameraRotation();
                isPinching = true;
            }
            else
            {
                float newPinchDistance = Vector2.Distance(inputManagerInstance.PrimaryFinger, inputManagerInstance.SecondaryFinger);
                zoomDelta = initialPinchDistance - newPinchDistance;

                initialPinchDistance = newPinchDistance;
            }
        }
        else
        {
            if (isPinching)
                EnableCameraRotation();
            isPinching = false;
        }

        targetZoomLevel += zoomDelta * zoomSpeed * Time.deltaTime;
        targetZoomLevel = Mathf.Clamp(targetZoomLevel, minZoom, maxZoom);


        if (freeLookCamera != null)
        {
            freeLookCamera.m_Lens.FieldOfView = Mathf.Lerp(
                    freeLookCamera.m_Lens.FieldOfView,
                    targetZoomLevel,
                    Time.deltaTime * zoomLerpSpeed
                    );
        }
    }

    public void DisableCameraRotation()
    {
        inputProvider.enabled = false;
    }


    public void EnableCameraRotation()
    {
        inputProvider.enabled = true;
    }
}

