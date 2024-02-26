using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class NewEyeInteractable : MonoBehaviour
{
    public bool IsHovered { get; set; }
    private MeshRenderer meshRenderer;
    [SerializeField] private UnityEvent<GameObject> OnObjectHover;
    [SerializeField] private Material OnHoverActiveMaterial;
    [SerializeField] private Material OnHoverInactiveMaterial;

    void Start() => meshRenderer = GetComponent<MeshRenderer>();

    // Update is called once per frame
    void Update()
    {
        if (IsHovered)
        { 
            meshRenderer.material = OnHoverActiveMaterial;
            OnObjectHover?.Invoke(gameObject);
        }
        else
        {
            meshRenderer.material = OnHoverInactiveMaterial;
        }
    }
}
