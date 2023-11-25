using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;

public class MentalCommands : MonoBehaviour
{
    EmotivUnityItf _emotivUnityltf = new EmotivUnityItf();
    bool mentalCmdRcvd = false;
    string mentalCommand;
    public Rigidbody ball;
    [SerializeField]
    float thrust = 20.0f;
    Vector3 forward = new Vector3(0, 0, 1);
    Vector3 pull = new Vector3(0, 0, -1);
    Vector3 right = new Vector3(1, 0, 0);
    Vector3 left = new Vector3(-1, 0, 0);

    List<string> dataStreamList = new List<string>() { DataStreamName.MentalCommands, DataStreamName.SysEvents };
    string clientId = "2BQ6yrpPyo00O3WRjZPQp18hd1roobOuF2cLxAVh";
    string clientSecret = "BcQrNVEj6YLavHmq4ihAWl2RE8noBU6lEk7IXrFOi59y0ws7nTmuNnYd1mlRxg10JihSMMtpFDnBxR3XDYHRG6f75OaGeBzyyefFk6w91uQw6zj0lORFeEzV3hknKDiU";
    string appName = "UnityApp";
    string profileName = "kyleSIM";
    string headsetId = "INSIGHT2-9FD8C3BB";
    // Start is called before the first frame update
    void Start()
    {
        _emotivUnityltf.Init(clientId,clientSecret,appName);
        _emotivUnityltf.Start();
        DataStreamManager.Instance.ScanHeadsets();
        ball = GetComponent<Rigidbody>();
        
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




        // Update is called once per frame
        void Update()
    {
        if (mentalCmdRcvd)
        {
            mentalCommand = _emotivUnityltf.mentalCmdIs();
            Debug.Log("Command is: " + mentalCommand);

            if (mentalCommand == "push")
            {
                ball.AddForce(forward * thrust);
            }

            if (mentalCommand == "pull")
            {
                ball.AddForce(pull * thrust);
            }

            if (mentalCommand == "right")
            {
                ball.AddForce(right * thrust);
            }

            if (mentalCommand == "left")
            {
                ball.AddForce(left * thrust);
            }
        }
        
    }
}
