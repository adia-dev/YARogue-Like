using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingTargetBehavior : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] private bool smoothRotation = false;
    [SerializeField] private float smoothRotationTime = 0.15f;
    [SerializeField] private bool inverse = false;

    float rotationSmoothVelocity;

    void Start()
    {

    }

    void Update()
    {
        if (target == null)
            return;

        Vector3 direction = target.position - transform.position;

        float targetRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + (inverse ? 180 : 0);
        if (smoothRotation)
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationSmoothVelocity, smoothRotationTime);
        else
            transform.eulerAngles = Vector3.up * targetRotation;
    }
}

