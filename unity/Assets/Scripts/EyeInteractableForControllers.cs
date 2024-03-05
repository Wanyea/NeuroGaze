using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractableForControllers : MonoBehaviour
{
    private Vector3 originalScale;
    private Vector3 targetScale; // Target scale when looked at
    public bool isHovered = false;
    private bool isBeingPulled = false;
    [SerializeField] private bool shouldShrink = false;
    [HideInInspector] public bool isPinching = false;
    private bool isShrinking = false; // New flag to indicate if the shrink process has started


    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale * 1.28f; // Set target scale as 1.28 times the original scale
    }

    public void Hover(bool state)
    {
        // Only allow hovering if the cube isn't already shrinking
        if (!isShrinking)
        {
            isHovered = state;
        }
    }

    private void Update()
    {
        if (isHovered)
        {
            if (isPinching)
            {
                shouldShrink = true;
                isBeingPulled = true;
                StartShrinking(); // Start shrinking and ignore further interactions
            }
        }
        else
        {
            // Reset logic if necessary, but ensure it doesn't interfere with ongoing shrinking
            if (isBeingPulled && !isShrinking)
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

    private void StartShrinking()
    {
        isShrinking = true;
        GetComponent<Collider>().enabled = false; // Disable the collider to prevent further ray interaction
        isHovered = false; // Optionally reset hover state
                           // Rest of the shrinking logic remains as is
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
        EyeInteractableManagerForControllers.Instance.NotifyCubeShrink(); // Existing logic

        float shrinkSpeed = 0.3f * Time.deltaTime;
        transform.localScale -= new Vector3(shrinkSpeed, shrinkSpeed, shrinkSpeed);

        if (transform.localScale.x <= 0.05f) // Threshold for destruction, adjust as needed
        {
            // Notify the Assessment Manager here before destroying the cube
            AssessmentManagerForControllers.Instance.RecordCubeDestruction(this.tag); // Assuming this.tag is "RedCube"
            Destroy(gameObject);
        }
    }

}
