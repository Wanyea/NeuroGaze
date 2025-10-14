using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EyeTrackingRayForHands : MonoBehaviour
{
    [SerializeField] private float rayDistance = 1000.0f;
    [SerializeField] private Transform leftEyeAnchor;
    [SerializeField] private Transform rightEyeAnchor;

    private LineRenderer lineRenderer;
    private EyeInteractableForHands lastEyeInteractableForHands;

    [SerializeField] private OVRHand leftHandUsedForPinchSelection;
    [SerializeField] private OVRHand rightHandUsedForPinchSelection;
    [SerializeField] private bool mockHandUsedForPinchSelection;

    public bool currentlyPinching = false;
    public int pinchCount = 0;
    public int maxPinch = 3;

    private bool allowPinchSelection;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupRay();
    }

    private void Start()
    {
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
        FrameCounter();

        Vector3 eyesCenter = (leftEyeAnchor.position + rightEyeAnchor.position) * 0.5f;
        Vector3 forwardDirection = (leftEyeAnchor.forward + rightEyeAnchor.forward) * 0.5f; 
        Ray ray = new Ray(eyesCenter, forwardDirection);
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit, rayDistance);


        if (isHit)
        {
            EyeInteractableForHands eyeInteractableForHands = hit.collider.GetComponent<EyeInteractableForHands>();

            if (pinchCount < 2) 
            {
                if (hit.transform.tag == "Wall" && IsPinching())
                {
                    AssessmentManagerForHands.Instance.errorCount++;
                }
            }

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

    private void SetLineColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    // Check whether or not a pinch is allowed and left or right hand is active
    private bool IsPinching()
    {
        if (allowPinchSelection && (pinchCount< maxPinch) && ((leftHandUsedForPinchSelection.GetFingerIsPinching(OVRHand.HandFinger.Index) ||
                                                          rightHandUsedForPinchSelection.GetFingerIsPinching(OVRHand.HandFinger.Index)) || mockHandUsedForPinchSelection))
        {     
            return true;
        } else
        {      
            return false;
        }
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