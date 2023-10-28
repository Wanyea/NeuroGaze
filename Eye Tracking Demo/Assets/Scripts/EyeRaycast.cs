using UnityEngine;
using Oculus.Platform;

public class EyeRaycast : MonoBehaviour
{
    public float maxRayDistance = 10.0f;
    public Color hitColor = Color.red;
    public GameObject leftEyeRay;
    public GameObject rightEyeRay;

    private MeshRenderer lastRenderer;
    private Color originalColor;

    void Start()
    {
        // Initialize the lines for visualizing the gaze
        leftEyeRay = new GameObject("LeftEyeRay");
        rightEyeRay = new GameObject("RightEyeRay");

        SetupEyeRay(leftEyeRay);
        SetupEyeRay(rightEyeRay);
    }

    void Update()
    {
        // Handle raycast based on main camera direction (since the true eye tracking API isn't accessible without the correct SDK or permissions)
        HandleEyeRaycast(Camera.main.transform.position, Camera.main.transform.forward, leftEyeRay);
        HandleEyeRaycast(Camera.main.transform.position, Camera.main.transform.forward, rightEyeRay);
    }

    private void HandleEyeRaycast(Vector3 origin, Vector3 direction, GameObject eyeRayObject)
    {
        Ray gazeRay = new Ray(origin, direction);
        RaycastHit hit;
        if (Physics.Raycast(gazeRay, out hit, maxRayDistance))
        {
            MeshRenderer renderer = hit.collider.GetComponent<MeshRenderer>();
            if (renderer)
            {
                if (lastRenderer && lastRenderer != renderer)
                {
                    lastRenderer.material.color = originalColor;
                    lastRenderer = null;
                }

                if (lastRenderer == null)
                {
                    originalColor = renderer.material.color;
                    renderer.material.color = hitColor;
                    lastRenderer = renderer;
                }
            }

            DrawEyeRay(eyeRayObject, origin, hit.point);
        }
        else
        {
            if (lastRenderer)
            {
                lastRenderer.material.color = originalColor;
                lastRenderer = null;
            }

            DrawEyeRay(eyeRayObject, origin, origin + direction * maxRayDistance);
        }
    }

    private void SetupEyeRay(GameObject eyeRayObject)
    {
        LineRenderer line = eyeRayObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Standard"));
        line.startWidth = 0.01f;
        line.endWidth = 0.01f;
    }

    private void DrawEyeRay(GameObject eyeRayObject, Vector3 start, Vector3 end)
    {
        LineRenderer line = eyeRayObject.GetComponent<LineRenderer>();
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }
}
