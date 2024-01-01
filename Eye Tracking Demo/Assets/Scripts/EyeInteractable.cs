using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    [SerializeField]
    private Material OnHoverActiveMaterial;
    [SerializeField]
    private Material OnMentalCommandActiveMaterial; 
    [SerializeField]
    private MentalCommands mentalCommands;
    [SerializeField]
    private float moveSpeed = 1.0f;

    private Material originalMaterial;
    private MeshRenderer meshRenderer;
    private Transform cameraTransform;
    private bool isHovered = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;
        cameraTransform = Camera.main.transform; 

        if (mentalCommands == null)
        {
            Debug.LogError("MentalCommands reference not set in EyeInteractable");
        }
    }

    private void Update()
    {
        if (isHovered) 
        {
            Debug.Log("Hoevered over cube!");
            var mentalCommand = mentalCommands.GetMentalCommand();

            /*// Check if connection with Emotiv has been established and mentals command are coming through
            if (mentalCommands.GetMentalCommand() != null) 
            {
                
                HandleMentalCommandMovement(mentalCommand);
                UpdateMaterialBasedOnMentalCommand(mentalCommand);
            }*/

        }
    }

    private void HandleMentalCommandMovement(string command)
    {
        
        Vector3 rayOrigin = Camera.main.transform.position; 
        Vector3 directionFromRayOrigin = (transform.position - rayOrigin).normalized;
        float step = moveSpeed * Time.deltaTime;

        switch (command)
        {
            case "pull":
            case "push":
                transform.position += command == "pull" ? -directionFromRayOrigin * step : directionFromRayOrigin * step;
                break;

            case "neutral":
            case "":
                // Stop movement
                break;
        }
    }

    private void UpdateMaterialBasedOnMentalCommand(string command)
    {
        if (command == "pull" || command == "push")
        {
            meshRenderer.material = OnMentalCommandActiveMaterial;
        }
        else
        {
            meshRenderer.material = originalMaterial;
        }
    }

    public void Hover(bool state)
    {
        isHovered = state;

        if (state)
        {
            // meshRenderer.material = OnHoverActiveMaterial;
        }
        else
        {
            // meshRenderer.material = originalMaterial;
        }
    }

    private void OnEnable()
    {
        if (mentalCommands != null)
        {
            mentalCommands.MentalCommandChanged += OnMentalCommandChanged;
        }
    }

    private void OnDisable()
    {
        if (mentalCommands != null)
        {
            mentalCommands.MentalCommandChanged -= OnMentalCommandChanged;
        }
    }

    private void OnMentalCommandChanged(string command)
    {
        if (isHovered)
        {
            HandleMentalCommandMovement(command);
            UpdateMaterialBasedOnMentalCommand(command);
        }
    }
}
