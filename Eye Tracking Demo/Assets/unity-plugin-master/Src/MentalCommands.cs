using System;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;

public class MentalCommands : MonoBehaviour
{
    EmotivUnityItf _emotivUnityltf = new EmotivUnityItf();
    bool mentalCmdRcvd = false;
    string mentalCommand;
    List<string> dataStreamList = new List<string>() { DataStreamName.MentalCommands, DataStreamName.SysEvents };
    string clientId = "wiAajbnzJ4EXPBCi5pczwdpofQIFsE8JhJxu5iKV";
    string clientSecret = "3P3UHnIHjzK5P1Hi5FgXrOTO6WqtecERe9mghtkzlhoD4lPvGaHEBmGthY931gWeho57NoCXYNa8pgocatBLpWjhBpemkMrF0DtMcj4bsj9gAU2ukAnGnsP1OSNzxhS9";
    string appName = "NeuroGaze";
    public string profileName = "wanyeaNewV2";
    public string headsetId = "INSIGHT2-A3D2036F";

    // Delegate for mental command changes
    public delegate void OnMentalCommandChanged(string newCommand);
    public event OnMentalCommandChanged MentalCommandChanged;


    void Start()
    {
        _emotivUnityltf.Init(clientId,clientSecret,appName);
        _emotivUnityltf.Start();
        DataStreamManager.Instance.ScanHeadsets();
    }

    private void OnGUI()
    {
        if (Event.current.Equals(Event.KeyboardEvent("return")))
        {
            _emotivUnityltf.CreateSessionWithHeadset(headsetId);
            UnityEngine.Debug.Log("Creating Session with: " + headsetId);


        }

        if (Event.current.Equals(Event.KeyboardEvent("l")))
        {
            _emotivUnityltf.LoadProfile(profileName);
            UnityEngine.Debug.Log("Loading Profile: " + profileName);

        }


        if (Event.current.Equals(Event.KeyboardEvent("s")))
        {
            _emotivUnityltf.SubscribeData(dataStreamList);
            UnityEngine.Debug.Log("Subscribing to DataStream");
            mentalCmdRcvd = true;
        }

        if (Event.current.Equals(Event.KeyboardEvent("escape")))
        {
            mentalCmdRcvd = false;
            _emotivUnityltf.UnLoadProfile(profileName);
            _emotivUnityltf.UnSubscribeData(dataStreamList);
            _emotivUnityltf.Stop();
            UnityEngine.Debug.Log("Unloaded Profile, Unsubscribed Data, and Stopped EmotivUnityItf");

        }


    }

    private string lastMentalCommand = "";

    void Update()
    {
        if (mentalCmdRcvd)
        {
            string currentCommand = _emotivUnityltf.mentalCmdIs();

            Debug.Log(mentalCommand);

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
}
