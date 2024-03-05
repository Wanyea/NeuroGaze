using System;
using System.IO;
using UnityEngine;

public class AssessmentManagerForHands : MonoBehaviour
{
    public static AssessmentManagerForHands Instance;

    [SerializeField] private string id;
    [SerializeField] string csvOutputPath;
    private int totalRedCubes;
    private int destroyedRedCubes;
    public int errorCount;
    private float startTime;
    private bool assessmentActive;

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

        csvOutputPath = "../unity/Assets/Assessment Results/Eye Gaze_Hands/StaticEyeGazeHandsResults_" + id + ".csv";
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
        string newLine = $"{id},{duration},{errorCount},{totalRedCubes}\n";

        if (!File.Exists(csvOutputPath))
        {
            File.WriteAllText(csvOutputPath, "ID,Round Duration,Error Count,Total Eye Interactables\n");
        }

        File.AppendAllText(csvOutputPath, newLine);

        // Reset the assessment
        EyeInteractableManagerForHands.Instance.ResetAssessment();
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