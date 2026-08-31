using System.Collections;
using TMPro;
using UnityEngine;

public class TrainingOrchestrator : MonoBehaviour
{
    [Header("Training Defaults")]
    [SerializeField] private float trainingWindowSeconds = 8f;
    [SerializeField] private int neutralReps = 10;
    [SerializeField] private int pullReps = 10;
    [SerializeField] private float restSecondsBetweenReps = 2f;

    [Header("Stimuli")]
    [Tooltip("Optional: stimulus during neutral training (e.g., look-around white cubes).")]
    [SerializeField] private GameObject neutralStimulusRoot;

    [Tooltip("Optional: stimulus during pull training (e.g., shrinking red cube).")]
    [SerializeField] private CubeTrainingStimulus pullStimulus;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI repText;

    private bool _isRunning;

    public void StartTrainingProgram()
    {
        if (_isRunning) return;
        StartCoroutine(RunTrainingProgram());
    }

    private IEnumerator RunTrainingProgram()
    {
        _isRunning = true;

        SetStatus("Initializing Emotiv...");
        yield return EmotivService.Instance.EnsureReadyForCommands();

        if (!EmotivService.Instance.IsReadyForCommands)
        {
            SetStatus("Emotiv not ready. Check headset + profile.");
            _isRunning = false;
            yield break;
        }

        // --- Neutral Block ---
        yield return RunBlock(action: "neutral", reps: neutralReps, stimulusMode: StimulusMode.Neutral);
        EmotivService.Instance.SaveProfile();
        SetStatus("Saved profile after Neutral block.");
        yield return new WaitForSeconds(1f);

        // --- Pull Block ---
        yield return RunBlock(action: "pull", reps: pullReps, stimulusMode: StimulusMode.Pull);
        EmotivService.Instance.SaveProfile();
        SetStatus("Saved profile after Pull block.");
        yield return new WaitForSeconds(1f);

        SetStatus("Training complete.");
        SetRep("");
        _isRunning = false;
    }

    private enum StimulusMode { Neutral, Pull, None }

    private IEnumerator RunBlock(string action, int reps, StimulusMode stimulusMode)
    {
        SetStatus($"Starting {action.ToUpper()} training block...");
        ActivateStimulus(stimulusMode, true);

        for (int i = 1; i <= reps; i++)
        {
            SetRep($"{action} rep {i}/{reps}");
            SetStatus("Get ready...");
            yield return new WaitForSeconds(1f);

            // Start training
            bool ok = EmotivService.Instance.StartTraining(action);
            if (!ok)
            {
                SetStatus($"Failed to start training '{action}'.");
                break;
            }

            // Run stimulus / hold window
            SetStatus($"Training '{action}' now ({trainingWindowSeconds:0}s)...");
            yield return new WaitForSeconds(trainingWindowSeconds);

            // Accept the training
            EmotivService.Instance.AcceptTraining();
            SetStatus("Accepted. Rest...");
            yield return new WaitForSeconds(restSecondsBetweenReps);
        }

        ActivateStimulus(stimulusMode, false);
        SetStatus($"{action.ToUpper()} block complete.");
    }

    private void ActivateStimulus(StimulusMode mode, bool active)
    {
        if (mode == StimulusMode.Neutral && neutralStimulusRoot != null)
            neutralStimulusRoot.SetActive(active);

        if (mode == StimulusMode.Pull && pullStimulus != null)
        {
            if (active) pullStimulus.StartLoop();
            else pullStimulus.StopLoop();
        }
    }

    private void SetStatus(string msg)
    {
        Debug.Log("[TrainingOrchestrator] " + msg);
        if (statusText != null) statusText.text = msg;
    }

    private void SetRep(string msg)
    {
        if (repText != null) repText.text = msg;
    }
}