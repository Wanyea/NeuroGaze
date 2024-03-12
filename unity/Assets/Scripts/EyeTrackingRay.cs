using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EyeTrackingRay : MonoBehaviour
{
    [SerializeField] private float rayDistance = 1000.0f;
    [SerializeField] private Transform leftEyeAnchor;
    [SerializeField] private Transform rightEyeAnchor;

    private LineRenderer lineRenderer;
    private EyeInteractable lastEyeInteractable;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupRay();
    }
    private void Start()
    {
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
        Vector3 eyesCenter = (leftEyeAnchor.position + rightEyeAnchor.position) * 0.5f; // Average the left and right eye anchors to find the midpoint
        Vector3 forwardDirection = (leftEyeAnchor.forward + rightEyeAnchor.forward) * 0.5f; // Average the forward direction of both eyes
        Ray ray = new Ray(eyesCenter, forwardDirection);
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit, rayDistance);

        // Check the users eye gaze has hit an object
        if (isHit)
        {
            EyeInteractable eyeInteractable = hit.collider.GetComponent<EyeInteractable>();

            // Check if the users eye gaze hit an item we can interact with (select)
            if (eyeInteractable)
            {
                if (lastEyeInteractable != eyeInteractable)
                {
                    if (lastEyeInteractable != null)
                    {
                        lastEyeInteractable.Hover(false); // If previous interactable object was in a hover state, set this state to false
                    }
                    lastEyeInteractable = eyeInteractable;
                }
                eyeInteractable.Hover(true);  // Enlargen or set this cubes "hover state" to true to let users know that can select it
            }
            else if (lastEyeInteractable != null)
            {
                lastEyeInteractable.Hover(false); // If previous interactable object was in a hover state, set this state to false
                lastEyeInteractable = null;
            }
        }
        else
        {
            if (lastEyeInteractable != null)
            {
                lastEyeInteractable.Hover(false); // If previous interactable object was in a hover state, set this state to false
                lastEyeInteractable = null;
            }
        }

        // Update the LineRenderer to represent the ray deriving from the users eye
        lineRenderer.SetPosition(0, eyesCenter);
        lineRenderer.SetPosition(1, eyesCenter + forwardDirection * rayDistance);
    }
}
