using UnityEngine;
using System.Collections;

public class CubeTrainingManager : MonoBehaviour
{
    [SerializeField] private GameObject cube; // Assign the cube GameObject in the inspector
    [SerializeField] private float duration = 8f; // Duration over which the cube shrinks
    [SerializeField] private bool startShrinking = false; // Toggle this in the inspector to start shrinking
    [SerializeField] private bool resetCube = false; // Toggle this in the inspector to reset cube size

    private Vector3 originalScale; // To store the original scale of the cube

    private void Start()
    {
        if (cube != null)
        {
            originalScale = cube.transform.localScale;
        }
        else
        {
            Debug.LogError("CubeTrainingManager: The cube object is not assigned.");
        }
    }

    private void Update()
    {
        if (startShrinking)
        {
            StartShrinking();
            startShrinking = false; // Reset the button
        }

        if (resetCube)
        {
            ResetCubeSize();
            resetCube = false; // Reset the button
        }
    }

    // Method to start the shrinking process
    public void StartShrinking()
    {
        StartCoroutine(ShrinkCubeOverTime(cube, duration));
    }

    // Coroutine that gradually scales down the cube
    private IEnumerator ShrinkCubeOverTime(GameObject obj, float time)
    {
        float currentTime = 0f;
        while (currentTime < time)
        {
            obj.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        }
        obj.transform.localScale = Vector3.zero;
    }

    // Method to reset the cube size
    public void ResetCubeSize()
    {
        if (cube != null)
        {
            cube.transform.localScale = originalScale;
        }
    }
}
