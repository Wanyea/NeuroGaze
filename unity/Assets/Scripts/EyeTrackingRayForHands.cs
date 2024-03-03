using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EyeTrackingRayForHands : MonoBehaviour
{
    [SerializeField] private float rayDistance = 1000.0f;
    [SerializeField] private Transform leftEyeAnchor;
    [SerializeField] private Transform rightEyeAnchor;

    private LineRenderer lineRenderer;

    [SerializeField] private OVRHand leftHandUsedForPinchSelection;
    [SerializeField] private OVRHand rightHandUsedForPinchSelection;
    [SerializeField] private bool mockHandUsedForPinchSelection;

    //[SerializeField] private GameObject eyeReticle;

    private EyeInteractableForHands lastEyeInteractableForHands;

    public bool currentlyPinching = false;
    public int pinchCount = 0;
    public int maxPinch = 3;
    public float offsetFromHit = 0.1f;
    public float positionUpdateThreshold = 0.02f; // Threshold for updating the reticle's position

    private bool allowPinchSelection;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupRay();
    }

    private void Start()
    {
/*        if (eyeReticle != null)
        {
            if (eyeReticle.scene.rootCount == 0)
            {
                eyeReticle = Instantiate(eyeReticle, new Vector3(0, 0, 0), Quaternion.identity);
            }
        }
        else
        {
            Debug.LogError("Eye Reticle GameObject is not assigned in the inspector!");
        }*/
        allowPinchSelection = (leftHandUsedForPinchSelection != null) || (rightHandUsedForPinchSelection != null);
    }

    private void SetupRay()
    {
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
    }

    private void FixedUpdate()
    {
        Vector3 eyesCenter = (leftEyeAnchor.position + rightEyeAnchor.position) * 0.5f;
        Vector3 forwardDirection = (leftEyeAnchor.forward + rightEyeAnchor.forward).normalized;
        Ray ray = new Ray(eyesCenter, forwardDirection);
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit, rayDistance);

        if (isHit)
        {
            // Calculate the adjusted position with the offset
            Vector3 adjustedPosition = hit.point + hit.normal * offsetFromHit;

            // Check if the distance between the current position and the new position exceeds the threshold
            if (Vector3.Distance(eyeReticle.transform.position, adjustedPosition) > positionUpdateThreshold)
            {
                Debug.Log($"Hit detected at {hit.point}, moving reticle.");
                eyeReticle.transform.position = adjustedPosition;
                eyeReticle.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            }
        }

        UpdateLineRenderer(eyesCenter, forwardDirection);
    }

    private void UpdateLineRenderer(Vector3 start, Vector3 direction)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, start + direction * rayDistance);
    }

    private void HandleEyeInteractable(EyeInteractableForHands interactable)
    {
        if (interactable)
        {
            interactable.isPinching = IsPinching();

            if (lastEyeInteractableForHands != interactable)
            {
                if (lastEyeInteractableForHands != null)
                {
                    lastEyeInteractableForHands.Hover(false);
                }
                lastEyeInteractableForHands = interactable;
                interactable.Hover(true);
            }
        }
        else if (lastEyeInteractableForHands != null)
        {
            lastEyeInteractableForHands.Hover(false);
            lastEyeInteractableForHands = null;
        }
    }
    private bool IsPinching()
    {
        return allowPinchSelection && (pinchCount < maxPinch) &&
               ((leftHandUsedForPinchSelection.GetFingerIsPinching(OVRHand.HandFinger.Index) ||
                 rightHandUsedForPinchSelection.GetFingerIsPinching(OVRHand.HandFinger.Index)) || mockHandUsedForPinchSelection);
    }

    private void FrameCounter()
    {
        if (rightHandUsedForPinchSelection.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            pinchCount++;
        }
        else
        {
            pinchCount = 0;
        }
    }
}