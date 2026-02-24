using UnityEngine;

public class EyeTrackingDebug : MonoBehaviour
{
    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log($"eyeTrackingSupported: {OVRPlugin.eyeTrackingSupported}");
        Debug.Log($"eyeTrackingEnabled:   {OVRPlugin.eyeTrackingEnabled}");
        Debug.Log($"hasEyeTrackingPerm?:  {OVRPermissionsRequester.IsPermissionGranted(OVRPermissionsRequester.Permission.EyeTracking)}");
#else
        Debug.Log("EyeTrackingDebug: Not running on-device (Android). Eye tracking may not report in Editor/PCVR.");
#endif
    }

    void Update()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (OVRPlugin.eyeTrackingEnabled)
        {
            var state = OVRPlugin.GetEyeGazesState();
            Debug.Log($"EyeGazesState valid: {state.EyeGazes[0].IsValid} / {state.EyeGazes[1].IsValid}");
        }
#endif
    }
}