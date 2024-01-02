using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    [SerializeField]
    private Material OnHoverMaterial;

    private Material originalMaterial;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;
    }

    public void Hover(bool state)
    {
        meshRenderer.material = state ? OnHoverMaterial : originalMaterial;
    }
}
