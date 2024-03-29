using System;
using System.IO;
using UnityEngine;

public class AssessmentManager : MonoBehaviour
{
    public static AssessmentManager Instance;

    [SerializeField] private string id;
    [SerializeField] string csvOutputPath;
    private int totalRedCubes;
    private int destroyedRedCubes;

    public int errorCount;
    private float startTime;
    private bool assessmentActive;
    public int errorCountForMisclicks;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start() 
    {
        if (String.IsNullOrEmpty(id))
            id = Guid.NewGuid().ToString("N");

        csvOutputPath = "../unity/Assets/Assessment Results/NeuroGaze/StaticNeuroGazeResults_" + id + ".csv";
    }

    public void CalculateTotalRedCubes()
    {
        totalRedCubes = GameObject.FindGameObjectsWithTag("RedCube").Length;
        Debug.Log("Red cubes: " + totalRedCubes);
    }

    public void StartAssessment()
    {
        // Remove the line that calculates totalRedCubes here
        destroyedRedCubes = 0;
        errorCount = 0;
        errorCountForMisclicks = 0;
        assessmentActive = true;
        startTime = Time.time;
    }
    public void RecordCubeDestruction(string tag)
    {
        if (!assessmentActive)
            return;

        if (tag == "RedCube")
        {
            Debug.Log("Destroying red cube...");
            destroyedRedCubes++;
            if (destroyedRedCubes == totalRedCubes)
                EndAssessment();
        }
        else
        {
            errorCount++;
        }
    }

    private void EndAssessment()
    {
        Debug.Log("Ending Assessment...");
        assessmentActive = false;
        float endTime = Time.time;
        float duration = endTime - startTime;
        string newLine = $"{id},{duration},{errorCountForMisclicks},{errorCount},{totalRedCubes}\n";

        if (!File.Exists(csvOutputPath))
        {
            File.WriteAllText(csvOutputPath, "ID,Round Duration,Error Count For Misclicks, Error Count For Wrong Selection ,Total Eye Interactables\n");
        }

        File.AppendAllText(csvOutputPath, newLine);

        // Reset the assessment
        EyeInteractableManager.Instance.ResetAssessment();
    }

    public void ResetAssessment()
    {
        // Logic to reset the assessment state
        totalRedCubes = 0;
        destroyedRedCubes = 0;
        errorCount = 0;
        assessmentActive = false;
    }

}