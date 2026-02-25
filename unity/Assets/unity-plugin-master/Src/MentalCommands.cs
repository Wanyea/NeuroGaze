using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;

public class MentalCommands : MonoBehaviour
{
    public static MentalCommands Instance;

    private EmotivUnityItf _emotiv = new EmotivUnityItf();

    private string mentalCommand = "";
    private string lastMentalCommand = "";

    private readonly List<string> dataStreamList = new List<string>()
    {
        DataStreamName.MentalCommands,
        DataStreamName.SysEvents
    };

    [Header("Emotiv App Credentials (do not ship secrets in client builds)")]
    [SerializeField] private string clientId = "b2gnA1MhxkdE831kmVGw5rnskKdMcbbMxnLHgYCH";
    [SerializeField] private string clientSecret = "jSgvPKLHJoLfAIZk1ge6ZYNEuDlILZmYABz3JcTgyI38lEXZBGpQqSiHf8ODOv9Tv6QogcjjUYPMPzQ934QuHk78udYVZoRUbKpppkXDoRpsKYeZefQ08wxJ40lhOFm4";
    [SerializeField] private string appName = "NeuroGaze";

    [Header("BCI Config")]
    public string profileName = "WanyeaATI";
    public string headsetId = "EPOCX-E5020D33";

    [Header("Startup")]
    [SerializeField] private float authorizeTimeoutSeconds = 20f;
    [SerializeField] private float scanTimeoutSeconds = 20f;
    [SerializeField] private float sessionTimeoutSeconds = 20f;
    [SerializeField] private float profileTimeoutSeconds = 20f;

    public bool IsReady { get; private set; } = false;

    // Delegate for mental command changes
    public delegate void OnMentalCommandChanged(string newCommand);
    public event OnMentalCommandChanged MentalCommandChanged;

    private Coroutine _startupRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _startupRoutine = StartCoroutine(StartupSequence());
    }

    private IEnumerator StartupSequence()
    {
        IsReady = false;

        // 1) Init + Start authorize flow
        try
        {
            _emotiv.Init(clientId, clientSecret, appName);
            _emotiv.Start();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            yield break;
        }

        // 2) Wait authorize done (IsAuthorizedOK set in OnLicenseValidTo)
        yield return WaitUntilTrue(
            condition: () => _emotiv.IsAuthorizedOK,
            timeoutSeconds: authorizeTimeoutSeconds,
            timeoutMsg: "[MentalCommands] Timed out waiting for authorization (IsAuthorizedOK)."
        );
        if (!_emotiv.IsAuthorizedOK) yield break;

        // 3) Scan headsets
        try
        {
            DataStreamManager.Instance.ScanHeadsets();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            yield break;
        }

        // 4) Wait until scan finished AND our headset is detected
        yield return WaitUntilTrue(
            condition: () => IsHeadsetDetected(headsetId),
            timeoutSeconds: scanTimeoutSeconds,
            timeoutMsg: $"[MentalCommands] Timed out waiting for headset '{headsetId}' to be detected."
        );
        if (!IsHeadsetDetected(headsetId)) yield break;

        // 5) Create session (SDK internally connects + creates session)
        try
        {
            _emotiv.CreateSessionWithHeadset(headsetId);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            yield break;
        }

        // 6) Wait session created
        yield return WaitUntilTrue(
            condition: () => _emotiv.IsSessionCreated,
            timeoutSeconds: sessionTimeoutSeconds,
            timeoutMsg: "[MentalCommands] Timed out waiting for session creation (IsSessionCreated)."
        );
        if (!_emotiv.IsSessionCreated) yield break;

        // 7) Load profile
        try
        {
            _emotiv.LoadProfile(profileName);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            yield break;
        }

        // 8) Wait profile loaded
        yield return WaitUntilTrue(
            condition: () => _emotiv.IsProfileLoaded,
            timeoutSeconds: profileTimeoutSeconds,
            timeoutMsg: $"[MentalCommands] Timed out waiting for profile '{profileName}' to load (IsProfileLoaded)."
        );
        if (!_emotiv.IsProfileLoaded) yield break;

        // 9) Subscribe data (now session is active + profile loaded)
        try
        {
            _emotiv.SubscribeData(dataStreamList);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            yield break;
        }

        IsReady = true;
        Debug.Log("[MentalCommands] Ready: authorized, session created, profile loaded, subscribed.");
    }

    private bool IsHeadsetDetected(string id)
    {
        // DataStreamManager maintains _detectedHeadsets via cortex query/scan.
        // Scan sets IsHeadsetScanning true and will flip false on scan finished.
        // We'll consider detected if it appears in the list (regardless of status).
        var list = DataStreamManager.Instance.GetDetectedHeadsets();
        if (list == null) return false;

        foreach (var h in list)
        {
            if (h != null && h.HeadsetID == id)
                return true;
        }
        return false;
    }

    private IEnumerator WaitUntilTrue(Func<bool> condition, float timeoutSeconds, string timeoutMsg)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - start < timeoutSeconds)
        {
            bool ok = false;
            try
            {
                ok = condition();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                yield break;
            }

            if (ok) yield break;
            yield return null;
        }

        Debug.LogWarning(timeoutMsg);
    }

    private void Update()
    {
        if (!IsReady) return;

        // mentalCmdIs returns latest "action" updated by OnMentalCommandReceived
        string currentCommand = _emotiv.mentalCmdIs();

        // Some SDK states return empty string when nothing is happening
        if (string.IsNullOrEmpty(currentCommand))
            return;

        if (currentCommand != lastMentalCommand)
        {
            mentalCommand = currentCommand;
            lastMentalCommand = currentCommand;

            Debug.Log($"[MentalCommands] New mental command: {mentalCommand}");
            MentalCommandChanged?.Invoke(mentalCommand);
        }
    }

    public string GetMentalCommand()
    {
        return mentalCommand;
    }

    private void OnApplicationQuit()
    {
        try
        {
            if (IsReady)
            {
                _emotiv.UnSubscribeData(dataStreamList);
            }

            // Your original code used UnLoadProfile(headsetId) — that is incorrect per SDK.
            // UnLoadProfile expects profileName.
            if (_emotiv.IsProfileLoaded)
            {
                _emotiv.UnLoadProfile(profileName);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}