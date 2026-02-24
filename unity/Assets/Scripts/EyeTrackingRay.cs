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

    private void SetupRay()
    {
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
    }

    private void Update()
    {
        if (leftEyeAnchor == null || rightEyeAnchor == null)
            return;

        // Eye gaze origin & direction (combined)
        Vector3 eyesCenter = (leftEyeAnchor.position + rightEyeAnchor.position) * 0.5f;

        Vector3 forwardDirection = (leftEyeAnchor.forward + rightEyeAnchor.forward) * 0.5f;
        forwardDirection.Normalize();

        Ray ray = new Ray(eyesCenter, forwardDirection);

        bool isHit = Physics.Raycast(ray, out RaycastHit hit, rayDistance);

        // Hover logic
        if (isHit)
        {
            EyeInteractable eyeInteractable = hit.collider.GetComponent<EyeInteractable>();

            if (eyeInteractable != null)
            {
                if (lastEyeInteractable != eyeInteractable)
                {
                    if (lastEyeInteractable != null)
                        lastEyeInteractable.Hover(false);

                    lastEyeInteractable = eyeInteractable;
                }

                eyeInteractable.Hover(true);
            }
            else
            {
                ClearHover();
            }
        }
        else
        {
            ClearHover();
        }

        // Misclick check (keep your existing logic)
        if (isHit)
        {
            if (hit.collider.CompareTag("Wall") && MentalCommands.Instance.GetMentalCommand() == "pull")
                AssessmentManager.Instance.errorCountForMisclicks++;
        }

        // Draw the actual gaze ray we used
        lineRenderer.SetPosition(0, eyesCenter);
        lineRenderer.SetPosition(1, eyesCenter + forwardDirection * rayDistance);
    }

    private void ClearHover()
    {
        if (lastEyeInteractable != null)
        {
            lastEyeInteractable.Hover(false);
            lastEyeInteractable = null;
        }
    }
}