using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    [SerializeField]
    private Material OnHoverMaterial;
    [SerializeField]
    private Material OnMentalCommandActiveMaterial;
    [SerializeField]
    private MentalCommands mentalCommands;
    [SerializeField]
    private float moveSpeed = 1.0f;

    private Material originalMaterial;
    private MeshRenderer meshRenderer;
    private bool isHovered = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;

        if (mentalCommands == null)
        {
            Debug.LogError("MentalCommands reference not set in EyeInteractable");
        }
    }

    public void Hover(bool state)
    {
        isHovered = state;
        UpdateMaterial();
    }

    private void Update()
    {
        if (isHovered)
        {
            var mentalCommand = mentalCommands.GetMentalCommand();
            HandleMentalCommandMovement(mentalCommand);
        }
    }

    private void HandleMentalCommandMovement(string command)
    {
        Vector3 directionToUser = (Camera.main.transform.position - transform.position).normalized;
        float step = moveSpeed * Time.deltaTime;

        switch (command)
        {
            case "pull":

                // Move towards the user
                transform.position += directionToUser * step;
                break;

            case "push":
                
                // Move away from the user
                transform.position -= directionToUser * step;
                break;

            case "neutral":
                
                // Stop movement immediately
                // We don't need to add anything here as the object will simply stop moving
                break;
        }
    }


    private void UpdateMaterial()
    {
        var mentalCommand = mentalCommands.GetMentalCommand();
        if (isHovered)
        {
            meshRenderer.material = OnHoverMaterial;
        }
        else if (mentalCommand == "pull" || mentalCommand == "push")
        {
            meshRenderer.material = OnMentalCommandActiveMaterial;
        }
        else
        {
            meshRenderer.material = originalMaterial;
        }
    }
}
