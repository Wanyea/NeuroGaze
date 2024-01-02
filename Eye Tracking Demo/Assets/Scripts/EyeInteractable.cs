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
    private float pullDuration = 0f;
    private bool shouldShrink = false;
    private float gazeDuration = 0f;
  
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
        var mentalCommand = mentalCommands.GetMentalCommand();

        // Check if the cube is being gazed at with the "pull" command
        if (isHovered && mentalCommand == "pull")
        {
            gazeDuration += Time.deltaTime;
            if (gazeDuration >= 2f)
            {
                shouldShrink = true;
            }
        }
        else
        {
            // Reset gaze duration if gaze or command is broken
            gazeDuration = 0f;
        }

        if (shouldShrink)
        {
            ShrinkAndDestroy();
        }
    }

    private void ShrinkAndDestroy()
    {
        // Shrink the cube
        float shrinkSpeed = 0.3f * Time.deltaTime; // Adjust shrink speed as needed
        transform.localScale -= new Vector3(shrinkSpeed, shrinkSpeed, shrinkSpeed);

        // Destroy cube if it is small enough
        if (transform.localScale.x <= 0.05f) // Threshold for destruction, adjust as needed
        {
            Destroy(gameObject);
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
                // transform.position += directionToUser * step;
                this.transform.localScale = Vector3.zero;
                break;

            case "push":

                // Move away from the user
                // transform.position -= directionToUser * step;
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


    private void HandleShrinking()
    {
        if (shouldShrink)
        {
            // Shrink the cube slowly
            float shrinkSpeed = 2.0f * Time.deltaTime; // Adjust shrink speed as needed
            transform.localScale = Vector3.Max(Vector3.zero, transform.localScale - new Vector3(shrinkSpeed, shrinkSpeed, shrinkSpeed));
        }
    }

}
