using System.Collections;
using UnityEngine;

public class CubeGameplay : MonoBehaviour
{
    [SerializeField] private GameObject leftWall;
    [SerializeField] private GameObject rightWall;
    [SerializeField] private GameObject frontWall;
    [SerializeField] private GameObject backWall;

    void Start()
    {
        StartCoroutine(ChangeRandomCubesColorAfterDelay(leftWall, 3.0f));
        StartCoroutine(ChangeRandomCubesColorAfterDelay(rightWall, 3.0f));
        StartCoroutine(ChangeRandomCubesColorAfterDelay(frontWall, 3.0f));
        StartCoroutine(ChangeRandomCubesColorAfterDelay(backWall, 3.0f));
    }

    IEnumerator ChangeRandomCubesColorAfterDelay(GameObject parentObject, float delay)
    {
        yield return new WaitForSeconds(delay);

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
            if (child != parentObject.transform && cubesColored < 4)
            {
                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.yellow;
                    cubesColored++;
                }
            }
        }
    }
}
