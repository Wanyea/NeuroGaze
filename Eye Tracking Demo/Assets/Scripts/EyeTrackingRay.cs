using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class EyeTrackingRay : MonoBehaviour
{
    [SerializeField]
    private float rayDistance = 500.0f;

    [SerializeField]
    private float rayWidth = 0.01f;

    [SerializeField]
    private LayerMask layersToInclude;

    private LineRenderer lineRenderer;
    private Dictionary<int, EyeInteractable> interactables = new Dictionary<int, EyeInteractable>();
    private EyeInteractable lastEyeInteractable;
    private Transform leftEyeAnchor;
    private Transform rightEyeAnchor;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupRay();

        // Get the eye anchors for more accurate raycast/line renderer start position 
        OVRCameraRig cameraRig = FindObjectOfType<OVRCameraRig>();
        if (cameraRig)
        {
            leftEyeAnchor = cameraRig.leftEyeAnchor;
            rightEyeAnchor = cameraRig.rightEyeAnchor;
        }
    }

    private void SetupRay()
    {
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = rayWidth;
        lineRenderer.endWidth = rayWidth;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, new Vector3(transform.position.x, transform.position.y, transform.position.z + rayDistance));
    }

    private void Update()
    {
        if (leftEyeAnchor == null || rightEyeAnchor == null) return;

        Vector3 eyesCenter = (leftEyeAnchor.position + rightEyeAnchor.position) * 0.5f;

        Vector3 rayDirection = transform.TransformDirection(Vector3.forward) * rayDistance;
        bool intercepting = Physics.Raycast(eyesCenter, rayDirection, out RaycastHit hit, Mathf.Infinity, layersToInclude);

        // Change the LineRenderer start position to the eyes center
        lineRenderer.SetPosition(0, transform.InverseTransformPoint(eyesCenter));

        if (intercepting)
        {
            if (!interactables.TryGetValue(hit.transform.gameObject.GetHashCode(), out EyeInteractable eyeInteractable))
            {
                eyeInteractable = hit.transform.GetComponent<EyeInteractable>();
                interactables.Add(hit.transform.gameObject.GetHashCode(), eyeInteractable);
            }

            var toLocalSpace = transform.InverseTransformPoint(hit.point); 
            lineRenderer.SetPosition(1, toLocalSpace); 

            if (lastEyeInteractable && lastEyeInteractable != eyeInteractable)
            {
                lastEyeInteractable.Hover(false);
            }

            eyeInteractable.Hover(true);
            lastEyeInteractable = eyeInteractable;
        }
        else
        {
            if (lastEyeInteractable)
            {
                lastEyeInteractable.Hover(false);
                lastEyeInteractable = null;
            }
            lineRenderer.SetPosition(1, new Vector3(0, 0, rayDistance));
        }
    }

}
