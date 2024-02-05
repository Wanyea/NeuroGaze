using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.0f;
    private Vector3 originalScale;
    private Vector3 targetScale; // Target scale when looked at
    private bool isHovered = false;
    private bool isBeingPulled = false;
    private float gazeDuration = 0f;
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
        string mentalCommand = EyeInteractableManager.Instance.CurrentMentalCommand;

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
        EyeInteractableManager.Instance.NotifyCubeShrink(); // Initiate the cooldown in the manager

        float shrinkSpeed = 0.3f * Time.deltaTime;
        transform.localScale -= new Vector3(shrinkSpeed, shrinkSpeed, shrinkSpeed);

        // Destroy cube if it is small enough
        if (transform.localScale.x <= 0.05f) // Threshold for destruction, adjust as needed
        {
            Destroy(gameObject);
        }
    }

}
