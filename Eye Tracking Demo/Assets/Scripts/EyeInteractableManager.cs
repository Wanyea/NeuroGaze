using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro; 

public class EyeInteractableManager : MonoBehaviour
{
    public static EyeInteractableManager Instance;
    [SerializeField] private MentalCommands mentalCommands; // Assuming this component exists to fetch commands
    [SerializeField] GameObject buttonUI;
    private TextMeshPro assessmentTextMesh;
    public TextMeshPro mentalCommandsLog;
    public string CurrentMentalCommand { get; private set; } = "neutral";
    private float cooldownTimer = 0f;
    [SerializeField] private GameObject leftWall;
    [SerializeField] private GameObject rightWall;
    [SerializeField] private GameObject frontWall;
    [SerializeField] private GameObject backWall;
    [SerializeField] public bool startAssessment = false; // Toggle this in the inspector to start
    [SerializeField] private bool resetAssessment = false; // Toggle this in the inspector to reset
    [SerializeField] private int cooldownDuration = 2; // Set countdown duration in the inspector
    [SerializeField] private int targetsPerWall = 1;

    private bool assessmentStarted = false;

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

    void Start() 
    {
        assessmentTextMesh = buttonUI.GetComponent<TextMeshPro>();
    }

    void Update()
    {
        mentalCommandsLog.text = $"The Current Mental Command is {CurrentMentalCommand}";

        //Debug.Log($"The Current Mental Command is {CurrentMentalCommand}");

        // if (Input.GetKeyDown("p")) { CurrentMentalCommand = "pull"; }
        // if (Input.GetKeyDown("n")) { CurrentMentalCommand = "neutral"; }

        if (cooldownTimer > 0)
        {
            // Debug.Log($"Cooldown for {cooldownTimer} more seconds...");
            cooldownTimer -= Time.deltaTime;
            CurrentMentalCommand = "neutral";
        }
        else
        {
            CurrentMentalCommand = mentalCommands.GetMentalCommand();
        }

        if (startAssessment && !assessmentStarted)
        {
            AssessmentManager.Instance.StartAssessment(); // Start the assessment
            StartCoroutine(StartAssessment());
            assessmentStarted = true;
        }

        if (resetAssessment)
        {
            AssessmentManager.Instance.ResetAssessment(); // Reset the assessment
            ResetAssessment();
            resetAssessment = false;
        }
    }

    public void setStartAssessment() { startAssessment = true; }
    public void setResetAssessment() { resetAssessment = true;  }
    public void NotifyCubeShrink() 
    {
        cooldownTimer = cooldownDuration; // Reset cooldown
    }

    IEnumerator StartAssessment()
    {
        for (int i = cooldownDuration; i > 0; i--)
        {
            assessmentTextMesh.text = $"Assessment starts in {i} seconds...";
            //Debug.Log("Countdown: " + i);
            yield return new WaitForSeconds(1f);
        }

        assessmentTextMesh.text = "Assessment Started!";

        //Debug.Log("Assessment Started!");

        // Start changing the color of random cubes
        StartCoroutine(ChangeRandomCubesColor(leftWall));
        StartCoroutine(ChangeRandomCubesColor(rightWall));
        StartCoroutine(ChangeRandomCubesColor(frontWall));
        StartCoroutine(ChangeRandomCubesColor(backWall));

        // Calculate total red cubes after they have been set
        yield return new WaitForSeconds(0.1f); // Small delay to ensure cubes are colored first
        AssessmentManager.Instance.CalculateTotalRedCubes();
        AssessmentManager.Instance.StartAssessment();
    }

    public void ResetAssessment()
    {
        startAssessment = false;
        assessmentStarted = false;

        assessmentTextMesh.text = "Assessment Menu";
        //Debug.Log("Assessment Reset");

        // Reset and re-enable all cubes
        ResetAndEnableCubes(leftWall);
        ResetAndEnableCubes(rightWall);
        ResetAndEnableCubes(frontWall);
        ResetAndEnableCubes(backWall);

        foreach (GameObject cube in disabledCubes)
        {
            if (cube != null)
            {
                cube.SetActive(true);
                cube.tag = "EyeInteractable";
                Renderer renderer = cube.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.white;
                }
            }
        }

        disabledCubes.Clear();
    }

    private List<GameObject> disabledCubes = new List<GameObject>();

    public void AddDisabledCube(GameObject cube)
    {
        if (!disabledCubes.Contains(cube))
        {
            Debug.Log("Adding cube to disable list...");
            disabledCubes.Add(cube);
        }
    }

    void ResetAndEnableCubes(GameObject parentObject)
    {
        if (parentObject == null)
        {
            Debug.LogError("Parent object is not assigned!");
            return;
        }

        // Reset the color and state of all cubes and re-enable them
        Transform[] children = parentObject.GetComponentsInChildren<Transform>(true); // Include inactive children
        foreach (Transform child in children)
        {
            if (child != parentObject.transform)
            {
                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.white; // Reset color to white
                    child.gameObject.tag = "EyeInteractable"; // Reset tag

                    // Check if the GameObject is inactive before re-enabling it
                    if (!child.gameObject.activeSelf)
                    {
                        child.gameObject.SetActive(true); // Re-enable the cube
                    }
                }
            }
        }
    }

    IEnumerator ChangeRandomCubesColor(GameObject parentObject)
    {
        if (parentObject == null)
        {
            Debug.LogError("Parent object is not assigned!");
            yield break;
        }

        // Get all child objects
        Transform[] children = parentObject.GetComponentsInChildren<Transform>();

        // Shuffle the array
        System.Random rng = new System.Random();
        int n = children.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Transform value = children[k];
            children[k] = children[n];
            children[n] = value;
        }

        // Change the color of the first 4 cubes
        int cubesColored = 0;
        foreach (Transform child in children)
        {
            if (child != parentObject.transform && cubesColored < targetsPerWall)
            {
                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.red;
                    child.gameObject.tag = "RedCube"; // Tag the cube as RedCube
                    cubesColored++;
                }
            }
        }
    }

    void ResetCubesColor(GameObject parentObject)
    {
        if (parentObject == null)
        {
            Debug.LogError("Parent object is not assigned!");
            return;
        }

        Transform[] children = parentObject.GetComponentsInChildren<Transform>();

        // Reset the color of all cubes and change their tag
        foreach (Transform child in children)
        {
            if (child != parentObject.transform)
            {
                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.white; // Reset color to white
                    child.gameObject.tag = "EyeInteractable"; // Reset tag
                }
            }
        }
    }

}
