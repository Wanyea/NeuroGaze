using System;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using TMPro;

public class MentalCommands : MonoBehaviour
{
    public static MentalCommands Instance;
    EmotivUnityItf _emotivUnityltf = new EmotivUnityItf();
    bool mentalCmdRcvd = false;
    string mentalCommand;
    List<string> dataStreamList = new List<string>() { DataStreamName.MentalCommands, DataStreamName.SysEvents };
    string clientId = "B6zjKZOBxIEhrUYVjyxW2yucPbLeyRbeaTss4hXh";
    string clientSecret = "d3mS1kYsG3tQSGijWQHxlKPypyszEkIKoPXOAskBqYqhBi9WaCUfaAqnT0d9BXZJdhWDdxf7YeHrmcK7UfJK4uWlKIjOyovWkQqQo1TvNCyfoVq1vwEdKYidvOWyR50G";
    string appName = "MSI3";
    public string profileName = "wanyeaPilot";
    public string headsetId = "EPOCX-E50208B2";

    // Delegate for mental command changes
    public delegate void OnMentalCommandChanged(string newCommand);
    public event OnMentalCommandChanged MentalCommandChanged;

    void Start()
    {
        try
        {
            _emotivUnityltf.Init(clientId, clientSecret, appName);
            _emotivUnityltf.Start();
            DataStreamManager.Instance.ScanHeadsets();
            try
            {
                _emotivUnityltf.CreateSessionWithHeadset(headsetId);
                _emotivUnityltf.LoadProfile(profileName);
                _emotivUnityltf.SubscribeData(dataStreamList);
            } catch (Exception e)
            {
                Debug.LogException(e);
            }
        } catch (Exception e)
        {
            Debug.LogException(e);
        }
    }


    private string lastMentalCommand = "";

    void Update()
    {
        if (mentalCmdRcvd)
        {
            string currentCommand = _emotivUnityltf.mentalCmdIs();

            Debug.Log($"The current mental command is: {currentCommand}");
            if (currentCommand != lastMentalCommand)
            {
                
                mentalCommand = currentCommand;
                lastMentalCommand = currentCommand;

                // Trigger the event
                MentalCommandChanged?.Invoke(mentalCommand);
            }
        }
    }

    public string GetMentalCommand() 
    {
        return mentalCommand;
    }

    private void OnApplicationQuit()
    {
        _emotivUnityltf.UnSubscribeData(dataStreamList);
        _emotivUnityltf.UnLoadProfile(headsetId);
    }
}
