using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EyeTrackingRayForHands : MonoBehaviour
{
    [SerializeField] private float rayDistance = 500.0f;
    [SerializeField] private Transform leftEyeAnchor;
    [SerializeField] private Transform rightEyeAnchor;

    private LineRenderer lineRenderer;
    private EyeInteractableForHands lastEyeInteractableForHands;

    [SerializeField] private OVRHand leftHandUsedForPinchSelection;
    [SerializeField] private OVRHand rightHandUsedForPinchSelection;
    [SerializeField] private bool mockHandUsedForPinchSelection;

    private bool intercepting;
    private bool allowPinchSelection;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        allowPinchSelection = (leftHandUsedForPinchSelection != null) || (rightHandUsedForPinchSelection != null);
        SetupRay();
    }

    private void SetupRay()
    {
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
    }

    private void Update()
    {
        Vector3 eyesCenter = (leftEyeAnchor.position + rightEyeAnchor.position) * 0.5f;
        Vector3 forwardDirection = (leftEyeAnchor.forward + rightEyeAnchor.forward) * 0.5f; // Average the forward direction of both eyes
        Ray ray = new Ray(eyesCenter, forwardDirection);
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit, rayDistance);

        if (isHit)
        {
            EyeInteractableForHands eyeInteractableForHands = hit.collider.GetComponent<EyeInteractableForHands>();
            if (eyeInteractableForHands)
            {
                eyeInteractableForHands.isPinching = IsPinching();

                if (lastEyeInteractableForHands != eyeInteractableForHands)
                {
                    if (lastEyeInteractableForHands != null)
                    {
                        lastEyeInteractableForHands.Hover(false);
                    }
                    lastEyeInteractableForHands = eyeInteractableForHands;
                }
                eyeInteractableForHands.Hover(true);
            }
            else if (lastEyeInteractableForHands != null)
            {
                lastEyeInteractableForHands.Hover(false);
                lastEyeInteractableForHands = null;
            }
        }
        else
        {
            if (lastEyeInteractableForHands != null)
            {
                lastEyeInteractableForHands.Hover(false);
                lastEyeInteractableForHands = null;
            }
        }

        // Update the LineRenderer to represent the ray
        lineRenderer.SetPosition(0, eyesCenter);
        lineRenderer.SetPosition(1, eyesCenter + forwardDirection * rayDistance);
    }

    // Check whether or not a pinch is allowed and left or right hand is active
    private bool IsPinching() => (allowPinchSelection && (leftHandUsedForPinchSelection.GetFingerIsPinching(OVRHand.HandFinger.Index) || 
                                                          rightHandUsedForPinchSelection.GetFingerIsPinching(OVRHand.HandFinger.Index))) || mockHandUsedForPinchSelection;
}