using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ControllerTrackingRay : MonoBehaviour
{
    [SerializeField] private float rayDistance = 1000.0f;

    private LineRenderer lineRenderer;
    private EyeInteractableForHands lastInteractable;

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
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit, rayDistance);

        if (isHit)
        {
            EyeInteractableForHands interactable = hit.collider.GetComponent<EyeInteractableForHands>();

            if (interactable)
            {

                if (interactable != lastInteractable)
                {
                    if (lastInteractable != null)
                    {
                        lastInteractable.Hover(false);
                    }
                    lastInteractable = interactable;
                    interactable.Hover(true);
                }

                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
                {
                   interactable.isPinching = true;
                }
            } else if (lastInteractable != null)
            {
                lastInteractable.Hover(false);
                lastInteractable = null;
            }
        }
        else
        {
            if (lastInteractable != null)
            {
                lastInteractable.Hover(false);
                lastInteractable = null;
            }
        }

        // Update the LineRenderer to represent the ray
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + transform.forward * rayDistance);
    }
}
