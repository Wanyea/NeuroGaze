using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    private Vector3 originalScale;
    private Vector3 targetScale; // Target scale when looked at
    private bool isHovered = false;
    private bool isBeingPulled = false;
    [SerializeField] private bool shouldShrink = false;

    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale * 1.28f; // Set target scale as 1.28 times the original scale
    }

    public void Hover(bool state)
    {
        isHovered = state;
    }

    private void Update()
    {
        string mentalCommand = EyeInteractableManager.Instance.CurrentMentalCommand; // Reference to EyeInteractableManager Instance

        // Check hover state and current mental command
        if (isHovered && mentalCommand == "pull")
        {

          // Set flags to select and destroy cube, stop cube from being interacted with
           shouldShrink = true;
           isBeingPulled = true;
          
        }
        else
        {
            if (isBeingPulled)
            {
                transform.localScale = originalScale;
                isBeingPulled = false;
            }
        }

        // Check mental command and hover state triggers 
        if (shouldShrink)
        {
            ShrinkAndDestroy();
        }
        else if (isHovered)
        {
            ScaleUp(); // Change interactable scale to show user they are looking at it
        }
        else
        {
            ScaleDown(); // Change interactable scale to show user they are not looking at it
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
        EyeInteractableManager.Instance.NotifyCubeShrink(); // Initiate the cooldown in the manager

        float shrinkSpeed = 0.3f * Time.deltaTime;
        transform.localScale -= new Vector3(shrinkSpeed, shrinkSpeed, shrinkSpeed);

        // Destroy cube if it is small enough
        if (transform.localScale.x <= 0.05f) // Threshold for destruction, adjust as needed
        {
            // Notify the Assessment Manager here before destroying the cube
            AssessmentManager.Instance.RecordCubeDestruction(this.tag); // Assuming this.tag is "RedCube"
            Destroy(gameObject);
        }
    }

}
