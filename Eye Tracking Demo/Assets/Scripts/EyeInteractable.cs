using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    [SerializeField]
    private Material OnHoverActiveMaterial;

    private Material originalMaterial;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;
    }

    public void Hover(bool state)
    {
        if (state)
        {
            meshRenderer.material = OnHoverActiveMaterial;
        }
        else
        {
            meshRenderer.material = originalMaterial;
        }
    }
}
