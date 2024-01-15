using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    [SerializeField]
    private MentalCommands mentalCommands;
    [SerializeField]
    private float moveSpeed = 1.0f;

    private Vector3 originalScale;
    private Vector3 targetScale; // Target scale when looked at
    private bool isHovered = false;
    private bool isBeingPulled = false;
    private float gazeDuration = 0f;
    private bool shouldShrink = false;
    private float cooldownTimer = 0f; // Cooldown timer after a cube has been shrunk
    private const float CooldownDuration = 1.0f; // Cooldown duration in seconds

    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale * 1.28f; // Set target scale as 1.25 times the original scale
    }

    public void Hover(bool state)
    {
        isHovered = state;
    }

    private void Update()
    {
        // Check if the cooldown is active
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            return; // Skip the rest of the update if cooldown is active
        }

        var mentalCommand = mentalCommands.GetMentalCommand();

        if (isHovered && mentalCommand == "pull")
        {
            gazeDuration += Time.deltaTime;
            if (gazeDuration >= 0.0f)
            {
                shouldShrink = true;
                isBeingPulled = true;
            }
        }
        else
        {
            gazeDuration = 0f;
            if (isBeingPulled)
            {
                transform.localScale = originalScale;
                isBeingPulled = false;
            }
        }

        if (shouldShrink)
        {
            ShrinkAndDestroy();
        }
        else if (isHovered)
        {
            ScaleUp();
        }
        else
        {
            ScaleDown();
        }
    }

    private void ScaleUp()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 2);
    }

    private void ScaleDown()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 2);
    }

    private void ShrinkAndDestroy()
    {
        float shrinkSpeed = 0.3f * Time.deltaTime;
        transform.localScale -= new Vector3(shrinkSpeed, shrinkSpeed, shrinkSpeed);

        // Destroy cube if it is small enough
        if (transform.localScale.x <= 0.05f) // Threshold for destruction, adjust as needed
        {
            Destroy(gameObject);
            cooldownTimer = CooldownDuration; // Start the cooldown
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

    private void HandleShrinking()
    {
        if (shouldShrink)
        {
            // Shrink the cube slowly
            float shrinkSpeed = 2.0f * Time.deltaTime; 
            transform.localScale = Vector3.Max(Vector3.zero, transform.localScale - new Vector3(shrinkSpeed, shrinkSpeed, shrinkSpeed));
        }
    }

    private void OnDisable()
    {
        Debug.Log("Disabling Cube...");
        AssessmentManager.Instance.RecordCubeDestruction(GetComponent<Renderer>().material.color);
        EyeInteractableManager.Instance.AddDisabledCube(gameObject);

    }

}
