using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EyeTrackingRay : MonoBehaviour
{
    [SerializeField] private float rayDistance = 500.0f;
    [SerializeField] private Transform leftEyeAnchor;
    [SerializeField] private Transform rightEyeAnchor;   

    private LineRenderer lineRenderer;
    private EyeInteractable lastEyeInteractable;

    private void Start()
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
        Vector3 eyesCenter = (leftEyeAnchor.position + rightEyeAnchor.position) * 0.5f;
        Ray ray = new Ray(eyesCenter, transform.forward);
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit, rayDistance);

        if (isHit)
        {
            EyeInteractable eyeInteractable = hit.collider.GetComponent<EyeInteractable>();
            if (eyeInteractable)
            {
                if (lastEyeInteractable != eyeInteractable)
                {
                    if (lastEyeInteractable != null)
                    {
                        lastEyeInteractable.Hover(false);
                    }
                    lastEyeInteractable = eyeInteractable;
                }
                eyeInteractable.Hover(true);
            }
            else if (lastEyeInteractable != null)
            {
                lastEyeInteractable.Hover(false);
                lastEyeInteractable = null;
            }
        }
        else
        {
            if (lastEyeInteractable != null)
            {
                lastEyeInteractable.Hover(false);
                lastEyeInteractable = null;
            }
        }

        // Update the LineRenderer to represent the ray
        lineRenderer.SetPosition(0, eyesCenter);
        lineRenderer.SetPosition(1, eyesCenter + transform.forward * rayDistance);
    }
}
