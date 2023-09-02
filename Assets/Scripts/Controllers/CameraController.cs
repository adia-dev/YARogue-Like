using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
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
        // Touch-based Zoom
        // Handle zoom
        float zoomDelta = 0.0f;

        if (inputManagerInstance.SecondaryFingerTouched)
        {
            DisableCameraRotation();
            if (!isPinching)
            {
                // This is the start of the pinch
                initialPinchDistance = Vector2.Distance(inputManagerInstance.PrimaryFinger, inputManagerInstance.SecondaryFinger);
                isPinching = true;
            }
            else
            {
                // Compute new pinch distance and determine zoomDelta
                float newPinchDistance = Vector2.Distance(inputManagerInstance.PrimaryFinger, inputManagerInstance.SecondaryFinger);
                zoomDelta = initialPinchDistance - newPinchDistance;

                // Update initialPinchDistance for next frame
                initialPinchDistance = newPinchDistance;
            }
        }
        else
        {
            if (isPinching)
                EnableCameraRotation();
            isPinching = false;
        }

        HandleZoomCamera(zoomDelta * zoomSpeed * Time.deltaTime);

    }

    private void HandleZoomCamera(float deltaZoom)
    {
        if (freeLookCamera)
        {
            targetZoomLevel += deltaZoom;
            targetZoomLevel = Mathf.Clamp(targetZoomLevel, minZoom, maxZoom);


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

