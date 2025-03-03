using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

/// <summary>
/// Handles the VN Camera
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Infos")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private AnimationCurve shakingCurve;

    public bool atTargetPosition
    {
        get
        {
            return cameraTransform.position.Equals(targetPosition);
        }
    }

    public bool atTargetRotation
    {
        get
        {
            return Quaternion.Angle(cameraTransform.rotation, targetRotationQuaternion) <= 0.01f;

        }
    }

    public bool isShaking { get; private set; }
    public Vector3 targetPosition { get; private set; }
    public Vector3 targetRotation { get; private set; }
    private Quaternion targetRotationQuaternion;

    private float shakingCurrentTime;
    private float shakingTotalTime;

    public static CameraController instance;


    void Awake()
    {
        instance = this;
        SetPosition(new Vector3(0, 0, -10), true);
        SetRotation(Vector3.zero, true);
    }

    /// <summary>
    /// Changes the camera's position
    /// </summary>
    /// <param name="newPosition">The new position</param>
    /// <param name="immediate">Should the change be immediate ?</param>
    public void SetPosition(Vector3 newPosition, bool immediate = false)
    {
        targetPosition = newPosition;

        if (immediate)
        {
            cameraTransform.position = newPosition;
        }
    }

    /// <summary>
    /// Changes the camera's rotation
    /// </summary>
    /// <param name="newRotation">The new rotation</param>
    /// <param name="immediate">Should the change be immediate ?</param>
    public void SetRotation(Vector3 newRotation, bool immediate = false)
    {
        targetRotation = newRotation;
        targetRotationQuaternion = Quaternion.Euler(newRotation);

        if (immediate)
        {
            cameraTransform.eulerAngles = newRotation;
        }
    }

    /// <summary>
    /// Starts shaking the screen
    /// </summary>
    /// <param name="duration">The shaking duration</param>
    public void SetShaking(float duration)
    {
        shakingTotalTime = duration;
        shakingCurrentTime = 0;
        isShaking = true;
    }


    void Update()
    {
        if (isShaking)
        {
            shakingCurrentTime += Time.deltaTime;
            float strength = shakingCurve.Evaluate(shakingCurrentTime / shakingTotalTime);
            transform.position += Random.insideUnitSphere * strength;

            if (shakingCurrentTime >= shakingTotalTime) isShaking = false;
        }

        if (!atTargetPosition)
        {
            cameraTransform.position = Vector3.MoveTowards(
                cameraTransform.position,
                targetPosition,
                moveSpeed * Time.deltaTime);
        }

        if (!atTargetRotation)
        {
            cameraTransform.rotation = Quaternion.RotateTowards(
                cameraTransform.rotation,
                targetRotationQuaternion,
                rotationSpeed * Time.deltaTime);
        }
    }
}
