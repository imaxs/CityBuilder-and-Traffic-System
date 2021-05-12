using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    [System.Serializable]
    public struct CameraSettings
    {
        public float zoomSpeed;
        public float zoomNearest;
        public float zoomFarthest;
        public float rotateSpeed;
        public float panSpeed;
    }

    //
    // Configurable Parameters
    //
    public Camera camera = null;
    public CameraSettings cameraSettings;

    //
    // Internal Variables
    //
    private bool isRigLocked = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialZoomDistance;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 targetZoomDistance;


    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialZoomDistance = camera.transform.localPosition;

        targetPosition = initialPosition;
        targetRotation = initialRotation;
        targetZoomDistance = initialZoomDistance;
    }

    private void Update()
    {
        //
        // Chase Zoom Goal
        //
        bool zoomGoalReached = Vector3.Distance(camera.transform.localPosition, targetZoomDistance) < 0.1f;
        if (!zoomGoalReached)
            camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, targetZoomDistance, cameraSettings.zoomSpeed * Time.deltaTime);

        //
        // Chase Rotation Goal
        //
        bool rotationGoalReached = Mathf.Abs(transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y) < 0.5f;
        if (!rotationGoalReached)
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, cameraSettings.rotateSpeed * Time.deltaTime);

        //
        // Chase Position Goal
        //
        bool positionGoalReached = Vector3.Distance(transform.position, targetPosition) < 0.1f;
        if (!positionGoalReached)
            transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSettings.panSpeed * camera.transform.localPosition.magnitude * Time.deltaTime);

        if (zoomGoalReached && rotationGoalReached && positionGoalReached)
            isRigLocked = false;
    }

    public bool Active
    {
        get
        {
            return camera.enabled;
        }
        set
        {
            gameObject.SetActive(value);
            camera.enabled = value;
        }
    }

    public void ResetCameraRig()
    {
        isRigLocked = true;
        targetPosition = initialPosition;
        targetRotation = initialRotation;
        targetZoomDistance = initialZoomDistance;
    }

    public void ResetOrientation()
    {
        isRigLocked = true;
        targetRotation = initialRotation;
        targetZoomDistance = initialZoomDistance;
    }

    public void Zoom(float zoomAxis)
    {
        if (isRigLocked)
            return;

        float newTarget = camera.transform.localPosition.y - (zoomAxis * cameraSettings.zoomSpeed);
        SetTargetZoomDistance(newTarget);
    }

    public void Rotate(float rotateAxis)
    {
        if (isRigLocked)
            return;

        var newTarget = transform.rotation * Quaternion.AngleAxis(rotateAxis * cameraSettings.rotateSpeed, Vector3.up);
        SetTargetRotation(newTarget);
    }

    public void Pan(Vector2 panAxis)
    {
        if (isRigLocked)
            return;

        var panDirection = transform.rotation * new Vector3(panAxis.x, 0, panAxis.y);
        var newTarget = transform.position + panDirection * cameraSettings.panSpeed;
        SetTargetPosition(newTarget);
    }

    public void SetTargetGoal(Vector3 newPosition, Quaternion newRotation, float newZoomDistance)
    {
        if (isRigLocked)
            return;

        SetTargetPosition(newPosition);
        SetTargetRotation(newRotation);
        SetTargetZoomDistance(newZoomDistance);
    }

    private void SetTargetPosition(Vector3 newTarget)
    {
        targetPosition.x = newTarget.x;
        targetPosition.y = 0;
        targetPosition.z = newTarget.z;

        //
        // TODO: Clamp to world
        //
        //var terrainStart = Vector3.zero;
        //var terrainEnd = Vector3.one;
        //targetPosition.x = Mathf.Clamp(targetPosition.x, terrainStart.x, terrainEnd.x);
        //targetPosition.y = Mathf.Clamp(targetPosition.y, terrainStart.y, terrainEnd.y);
        //targetPosition.z = Mathf.Clamp(targetPosition.z, terrainStart.z, terrainEnd.z);
    }

    private void SetTargetRotation(Quaternion newTarget)
    {
        targetRotation.x = 0;
        targetRotation.z = 0;
        targetRotation.y = newTarget.y;
        targetRotation.w = newTarget.w;
    }

    private void SetTargetZoomDistance(float newTarget)
    {
        targetZoomDistance.x = 0;
        targetZoomDistance.y = Mathf.Clamp(newTarget, cameraSettings.zoomNearest, cameraSettings.zoomFarthest);
        targetZoomDistance.z = -Mathf.Clamp(newTarget, cameraSettings.zoomNearest, cameraSettings.zoomFarthest);
    }
}
