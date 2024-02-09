using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro; 

public class EyeInteractableManager : MonoBehaviour
{
    public static EyeInteractableManager Instance;
    [SerializeField] private MentalCommands mentalCommands; 
    public TextMeshPro mentalCommandsLog;
    public string currentMentalCommand { get; private set; } = "neutral";
    private float cooldownTimer = 0f;
    [SerializeField] private int cooldownDuration = 2; 
    public int currentIndex = 0;
    public List<GameObject> particleObjects;


    private int lastActiveIndex = -1;

    void Start()
    {
        // Ensure all particle systems are initially stopped
        StopAllParticleSystems();

        // Automatically start the particle system at index 0, assuming "neutral" is the initial state
        StartParticleSystemAtIndex(0);
        lastActiveIndex = 0; 
    }

    void Update()
    {
        string newMentalCommand = GetCurrentMentalCommand();

        // Check if the mental command has changed
        if (newMentalCommand != currentMentalCommand)
        {
 
            currentMentalCommand = newMentalCommand;

            currentIndex = GetIndexFromMentalCommand(currentMentalCommand);

            Debug.Log("Current Mental Command: " + currentMentalCommand + ", Current Index: " + currentIndex);

            StartParticleSystemAtIndex(currentIndex);

            if (lastActiveIndex >= 0 && lastActiveIndex != currentIndex)
            {
                StopParticleSystemAtIndex(lastActiveIndex);
            }

            lastActiveIndex = currentIndex;
        }
    }

    void StartParticleSystemAtIndex(int index)
    {
        if (index >= 0 && index < particleObjects.Count)
        {
            GameObject particleObject = particleObjects[index];
            ParticleSystem ps = particleObject.GetComponent<ParticleSystem>();
            if (particleObject != null)
            {
                particleObject.SetActive(true);

                if (ps != null && !ps.isPlaying)
                {
                    ps.Play();
                }
            }
        }
    }

    void StopParticleSystemAtIndex(int index)
    {
        if (index >= 0 && index < particleObjects.Count)
        {
            GameObject particleObject = particleObjects[index];
            ParticleSystem ps = particleObject.GetComponent<ParticleSystem>();
            if (ps != null && ps.isPlaying)
            {
                ps.Stop();
            }

            if (particleObject != null)
            {
                particleObject.SetActive(false); 
            }
        }
    }

    string GetCurrentMentalCommand()
    {
        if (mentalCommands == null)
        {
            Debug.LogError("mentalCommands is null!");
            return ""; 
        }
        return mentalCommands.GetMentalCommand();
    }

 
    int GetIndexFromMentalCommand(string command)
    {
        if (string.IsNullOrEmpty(command)) return -1; 

        switch (command.ToLower())
        {
            case "neutral":
                return 0;
            case "pull":
                return 1;
            case "push":
                return 2;
            default:
                return -1; 
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void StopAllParticleSystems()
    {
        // Iterate through all GameObjects in the particleObjects list and stop their ParticleSystem components
        foreach (GameObject particleObject in particleObjects)
        {
            ParticleSystem ps = particleObject.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop();
            }
        }
    }

}
