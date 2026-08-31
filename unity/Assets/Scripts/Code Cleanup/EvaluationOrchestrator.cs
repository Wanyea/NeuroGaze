using System.Collections;
using TMPro;
using UnityEngine;

public class EvaluationOrchestrator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EyeInteractableManager eyeInteractableManager;
    [SerializeField] private TextMeshProUGUI statusText;

    private IEnumerator Start()
    {
        SetStatus("Initializing Emotiv for evaluation...");
        yield return EmotivService.Instance.EnsureReadyForCommands();

        if (!EmotivService.Instance.IsReadyForCommands)
        {
            SetStatus("Emotiv not ready. Check headset + profile.");
            yield break;
        }

        SetStatus("Ready. Starting evaluation...");

        // Start evaluation without requiring any training scene to have happened.
        if (eyeInteractableManager != null)
        {
            eyeInteractableManager.setStartAssessment();
        }
        else
        {
            Debug.LogWarning("[EvaluationOrchestrator] EyeInteractableManager reference not set.");
        }
    }

    private void SetStatus(string msg)
    {
        Debug.Log("[EvaluationOrchestrator] " + msg);
        if (statusText != null) statusText.text = msg;
    }
}