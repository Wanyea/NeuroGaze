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
    string clientId = "2BQ6yrpPyo00O3WRjZPQp18hd1roobOuF2cLxAVh";
    string clientSecret = "BcQrNVEj6YLavHmq4ihAWl2RE8noBU6lEk7IXrFOi59y0ws7nTmuNnYd1mlRxg10JihSMMtpFDnBxR3XDYHRG6f75OaGeBzyyefFk6w91uQw6zj0lORFeEzV3hknKDiU";
    string appName = "UnityApp";
    string profileName = "kyleSIM";
    string headsetId = "INSIGHT2-1C816C02";

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
            if (currentCommand != lastMentalCommand)
            {
                mentalCommand = currentCommand;
                lastMentalCommand = currentCommand;
                Debug.Log(mentalCommand);

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
