using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ControllerTrackingRay : MonoBehaviour
{
    [SerializeField] private float rayDistance = 1000.0f;

    private LineRenderer lineRenderer;
    private EyeInteractableForControllers lastInteractable;

    // Define colors for the LineRenderer
    [SerializeField] private Material defaultColor;
    [SerializeField] private Material activeColor;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetLineColor(defaultColor.color); 
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
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit, rayDistance);

        if (isHit)
        {
            EyeInteractableForControllers interactable = hit.collider.GetComponent<EyeInteractableForControllers>();

     
                if (hit.transform.tag == "Wall")
                {
                    if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger) || OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
                    {
                        AssessmentManagerForControllers.Instance.errorCount++;
                    }
                }
        

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

                if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger) || OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
                {
                    interactable.isPinching = true;
                    Debug.Log($"IsHover: {interactable.isHovered}, IsPinching: {interactable.isPinching}");
                } else
                {
                    SetLineColor(defaultColor.color); // Change color back to white when trigger is released
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
    private void SetLineColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }
}