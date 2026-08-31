using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;

public class EmotivService : MonoBehaviour
{
    public static EmotivService Instance { get; private set; }

    [Header("Emotiv App Credentials (DO NOT SHIP SECRETS IN CLIENT BUILDS)")]
    [SerializeField] private string clientId = "YOUR_CLIENT_ID";
    [SerializeField] private string clientSecret = "YOUR_CLIENT_SECRET";
    [SerializeField] private string appName = "NeuroGaze";
    [SerializeField] private string appVersion = "1.0.0";

    [Header("Profile")]
    [SerializeField] private string profileName = "WanyeaATI";
    public string ProfileName => profileName;

    [Header("Headset Selection")]
    [SerializeField] private bool autoPickFirstHeadset = true;

    // Inspector dropdown will edit this
    [SerializeField] private int selectedHeadsetIndex = 0;

    // Read-only list (inspector shows but user selects via dropdown)
    [SerializeField, Tooltip("Populated after scan. Use the dropdown above to select.")]
    private List<string> detectedHeadsetIds = new List<string>();

    public bool AutoPickFirstHeadset => autoPickFirstHeadset;
    public IReadOnlyList<string> DetectedHeadsetIds => detectedHeadsetIds;
    public int SelectedHeadsetIndex => selectedHeadsetIndex;

    public string SelectedHeadsetId { get; private set; }

    [Header("Behavior")]
    [SerializeField] private float authorizeTimeoutSeconds = 20f;
    [SerializeField] private float scanTimeoutSeconds = 20f;
    [SerializeField] private float sessionTimeoutSeconds = 20f;
    [SerializeField] private float profileTimeoutSeconds = 20f;

    [Header("Streams")]
    [SerializeField] private List<string> streams = new List<string>
    {
        DataStreamName.MentalCommands,
        DataStreamName.SysEvents
    };

    public bool IsInitialized { get; private set; }
    public bool IsReadyForCommands { get; private set; }
    public string CurrentCommand { get; private set; } = "neutral";

    public event Action HeadsetsUpdated;

    private EmotivUnityItf _emotiv;
    private Coroutine _initRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _emotiv = EmotivUnityItf.Instance;
    }

    private void Start()
    {
        _initRoutine = StartCoroutine(InitializeRoutine());
    }

    public void Initialize()
    {
        if (_initRoutine != null) StopCoroutine(_initRoutine);
        _initRoutine = StartCoroutine(InitializeRoutine());
    }

    private IEnumerator InitializeRoutine()
    {
        IsInitialized = false;
        IsReadyForCommands = false;
        CurrentCommand = "neutral";

        try
        {
            _emotiv.Init(clientId, clientSecret, appName, appVersion, false);
            _emotiv.Start();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            yield break;
        }

        yield return WaitUntilTrue(() => _emotiv.IsAuthorizedOK, authorizeTimeoutSeconds,
            "[EmotivService] Timed out waiting for authorization.");
        if (!_emotiv.IsAuthorizedOK) yield break;

        IsInitialized = true;
        Debug.Log("[EmotivService] Authorized.");
    }

    /// <summary>
    /// Inspector button can call this.
    /// </summary>
    public void RescanHeadsets()
    {
        StartCoroutine(ScanHeadsetsRoutine());
    }

    public IEnumerator ScanHeadsetsRoutine()
    {
        if (!IsInitialized)
        {
            yield return WaitUntilTrue(() => IsInitialized, authorizeTimeoutSeconds,
                "[EmotivService] Init wait timed out.");
            if (!IsInitialized) yield break;
        }

        try
        {
            DataStreamManager.Instance.ScanHeadsets();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            yield break;
        }

        yield return WaitUntilTrue(() => GetDetectedHeadsetsInternal().Count > 0, scanTimeoutSeconds,
            "[EmotivService] Timed out waiting for any headset to be detected.");

        RefreshDetectedHeadsets();

        if (autoPickFirstHeadset && detectedHeadsetIds.Count > 0)
        {
            SetSelectedHeadsetIndex(0);
        }
        else
        {
            // keep current index if valid, otherwise clamp
            if (detectedHeadsetIds.Count > 0)
                SetSelectedHeadsetIndex(Mathf.Clamp(selectedHeadsetIndex, 0, detectedHeadsetIds.Count - 1));
        }
    }

    public void RefreshDetectedHeadsets()
    {
        detectedHeadsetIds.Clear();

        var list = GetDetectedHeadsetsInternal();
        foreach (var h in list)
        {
            if (h != null && !string.IsNullOrEmpty(h.HeadsetID))
                detectedHeadsetIds.Add(h.HeadsetID);
        }

        // keep selected id consistent
        if (detectedHeadsetIds.Count == 0)
        {
            SelectedHeadsetId = null;
            selectedHeadsetIndex = 0;
        }
        else
        {
            selectedHeadsetIndex = Mathf.Clamp(selectedHeadsetIndex, 0, detectedHeadsetIds.Count - 1);
            SelectedHeadsetId = detectedHeadsetIds[selectedHeadsetIndex];
        }

        HeadsetsUpdated?.Invoke();
    }

    /// <summary>
    /// Called by inspector dropdown changes.
    /// </summary>
    public void SetSelectedHeadsetIndex(int index)
    {
        if (detectedHeadsetIds == null || detectedHeadsetIds.Count == 0)
        {
            SelectedHeadsetId = null;
            selectedHeadsetIndex = 0;
            return;
        }

        selectedHeadsetIndex = Mathf.Clamp(index, 0, detectedHeadsetIds.Count - 1);
        SelectedHeadsetId = detectedHeadsetIds[selectedHeadsetIndex];
        Debug.Log($"[EmotivService] Selected headset: {SelectedHeadsetId}");
    }

    public IEnumerator EnsureReadyForCommands()
    {
        if (!IsInitialized)
        {
            yield return WaitUntilTrue(() => IsInitialized, authorizeTimeoutSeconds,
                "[EmotivService] Init wait timed out.");
            if (!IsInitialized) yield break;
        }

        if (string.IsNullOrEmpty(SelectedHeadsetId))
        {
            yield return ScanHeadsetsRoutine();
            if (string.IsNullOrEmpty(SelectedHeadsetId))
            {
                Debug.LogWarning("[EmotivService] No headset selected; cannot proceed.");
                yield break;
            }
        }

        if (!_emotiv.IsSessionCreated)
        {
            try { _emotiv.CreateSessionWithHeadset(SelectedHeadsetId); }
            catch (Exception e) { Debug.LogException(e); yield break; }

            yield return WaitUntilTrue(() => _emotiv.IsSessionCreated, sessionTimeoutSeconds,
                "[EmotivService] Timed out waiting for session creation.");
            if (!_emotiv.IsSessionCreated) yield break;
        }

        if (!_emotiv.IsProfileLoaded)
        {
            try { _emotiv.LoadProfile(profileName); }
            catch (Exception e) { Debug.LogException(e); yield break; }

            yield return WaitUntilTrue(() => _emotiv.IsProfileLoaded, profileTimeoutSeconds,
                $"[EmotivService] Timed out waiting for profile '{profileName}' to load.");
            if (!_emotiv.IsProfileLoaded) yield break;
        }

        try { _emotiv.SubscribeData(streams); }
        catch (Exception e) { Debug.LogException(e); yield break; }

        IsReadyForCommands = true;
        Debug.Log("[EmotivService] Ready for commands.");
    }

    public bool StartTraining(string action)
    {
        if (!_emotiv.IsProfileLoaded) return false;
        _emotiv.StartMCTraining(action);
        return true;
    }

    public bool AcceptTraining()
    {
        if (!_emotiv.IsProfileLoaded) return false;
        _emotiv.AcceptMCTraining();
        return true;
    }

    public bool SaveProfile()
    {
        if (!_emotiv.IsProfileLoaded) return false;
        _emotiv.SaveProfile(profileName);
        return true;
    }

    private void Update()
    {
        if (!IsReadyForCommands) return;
        var cmd = _emotiv.mentalCmdIs();
        if (!string.IsNullOrEmpty(cmd)) CurrentCommand = cmd;
    }

    private List<Headset> GetDetectedHeadsetsInternal()
    {
        return DataStreamManager.Instance.GetDetectedHeadsets() ?? new List<Headset>();
    }

    private IEnumerator WaitUntilTrue(Func<bool> condition, float timeoutSeconds, string timeoutMsg)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - start < timeoutSeconds)
        {
            bool ok = false;
            try { ok = condition(); }
            catch (Exception e) { Debug.LogException(e); yield break; }

            if (ok) yield break;
            yield return null;
        }

        Debug.LogWarning(timeoutMsg);
    }
}