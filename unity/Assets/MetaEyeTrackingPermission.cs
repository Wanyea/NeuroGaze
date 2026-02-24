using UnityEngine;

public class MetaEyeTrackingPermission : MonoBehaviour
{
    private void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // Eye Tracking permission gate
        if (!OVRPermissionsRequester.IsPermissionGranted(OVRPermissionsRequester.Permission.EyeTracking))
        {
            Debug.Log("[MetaEyeTrackingPermission] Requesting EyeTracking permission...");
            OVRPermissionsRequester.RequestPermission(OVRPermissionsRequester.Permission.EyeTracking);
        }
        else
        {
            Debug.Log("[MetaEyeTrackingPermission] EyeTracking permission already granted.");
        }
#endif
    }
}